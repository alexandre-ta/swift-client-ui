using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// Controller
    /// </summary>
    public class OpenStackController
    {
        /// <summary>
        /// Model
        /// </summary>
        private FormModel model;

        /// <summary>
        /// View
        /// </summary>
        private OpenStackView view;
        
        /// <summary>
        /// Tools for accessing the database
        /// </summary>
        private RestTools tools;
        
        /// <summary>
        /// Authentification URI
        /// </summary>
        private String auth_url;

        /// <summary>
        /// Authentification version
        /// </summary>
        private String auth_version;

        /// <summary>
        /// Storage URI
        /// </summary>
        private String storage_url;

        /// <summary>
        /// Storage version
        /// </summary>
        private String storage_version;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view"></param>
        public OpenStackController(FormModel model, OpenStackView view)
        {
            this.model = model;
            this.view = view;
            this.view.AddListener(this);
            Initialize();
        }

        /// <summary>
        /// Initialise this class
        /// </summary>
        public void Initialize()
        {
            // Initialize panel
            view.EnableBrowser(false);
            view.EnableLogin(false);
            view.EnableButtonsObjects(false);
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("configProject.xml");
                XmlNode nodeAuthentication = doc.SelectSingleNode("/Project/Configuration/Authentication");
                auth_url = nodeAuthentication.SelectSingleNode("Url").InnerText;
                auth_version = nodeAuthentication.SelectSingleNode("Version").InnerText;

                XmlNode nodeStorage = doc.SelectSingleNode("/Project/Configuration/Storage");
                storage_url = nodeStorage.SelectSingleNode("Url").InnerText;
                storage_version = nodeStorage.SelectSingleNode("Version").InnerText;

                tools = new RestTools(auth_url, auth_version, storage_url, storage_version);

                view.Url = String.Empty;

                Information("XML Config loaded successfully");
                view.EnableLogin(true);
            }
            catch (Exception)
            {
                Information("XML Config loaded failed\nPlease check the config file and re open this program.");
            }
        }

        /// <summary>
        /// Connect to the database with tenant name, username and password
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Connect(String tenant, String username, String password)
        {
            view.EnableBrowser(false);
            view.EnableButtonsObjects(false);
            if (tools.Connect(username, password))
            {
                if (tools.GetTokens(tenant, username, password))
                {
                    Information("Connected !");
                    GetContainers();
                    Information("Get Containers successfully!");
                    view.EnableBrowser(true);
                }
                else
                {
                    Information("Connection failed !\nPlease check your tenant, username and password.");
                }
            }
            else
            {
                Information("Connection failed !\nPlease check your username and password.");
                view.EnableBrowser(false);
            }
        }

        /// <summary>
        /// Retrieve all containers
        /// </summary>
        public void GetContainers()
        {
            view.EnableButtonsObjects(false);
            model.CurrentNode = null;
            model.Root = null;
            model.CurrentContainer = null;
            FilesModel tmp = tools.GetFiles("");
            for (int i = 0; i < tmp.GetFiles().Count(); i++ )
            {
                FileModel file = tmp.GetFiles().ElementAt(i);
                file.Name += "/";
                file.Uri = file.Name;
            }
            model.Containers = tmp;
            view.UpdateContainers(tmp);
            view.UpdateTree(null);
            GetUrl();
        }

        /// <summary>
        /// Select container with index
        /// </summary>
        /// <param name="index"></param>
        public void SelectContainer(int index)
        {
            FileModel currentContainer = model.Containers.GetFiles().ElementAt(index);
            model.CurrentContainer = currentContainer;
            model.Root = GetTree(model.CurrentContainer);
            model.CurrentNode = model.Root;
            view.UpdateTree(model.CurrentNode);
            view.EnableButtonsObjects(true);
            Information("Get objects from " + currentContainer.Name);
            GetUrl();
        }

        /// <summary>
        /// Get the tree of a container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public NodeModel GetTree(FileModel container)
        {
            FilesModel files = tools.GetFiles(container.Name);
            NodeModel root = new NodeModel(container);
            NodeModel current, tmp;

            for (int i = 0; i < files.GetFiles().Count(); i++)
            {
                FileModel file = files.GetFiles().ElementAt(i);
                String[] words = file.Name.Split('/');
                current = root;
                
                for(int j = 0; j < words.Count(); j ++)
                {
                    String nameWord = words[j];
                    if (nameWord != String.Empty)
                    {
                        Boolean isLastWord = (j == (words.Count() - 1));

                        if (!isLastWord)
                        {
                            nameWord += "/";
                        }
                        if ((tmp = current.FindNode(nameWord)) != null)
                        {
                            current = tmp;
                        }
                        else
                        {
                            FileModel f = new FileModel();
                            f.Name = nameWord;
                            if (isLastWord)
                            {
                                f.Uri = container.Name + file.Name;
                                f.IsDirectory = false;
                                f.Size = file.Size;
                                f.Hash = file.Hash;
                                f.Last_modified = file.Last_modified;
                            }
                            else
                            {
                                if (words[j + 1].Equals(String.Empty))
                                {
                                    f.Uri = container.Name + file.Name;
                                }
                                f.IsDirectory = true;
                            }
                            NodeModel temp = new NodeModel(f);
                            current.AddChild(temp);
                            current = temp;
                        }
                    }
                }
            }
            return root;
        }

        /// <summary>
        /// Access to child node by index
        /// </summary>
        /// <param name="index"></param>
        public void GoToFile(int index)
        {
            NodeModel tmp = model.CurrentNode;
            NodeModel node = tmp.Children[index];
            if (node.Value.IsDirectory)
            {
                // Execute Folder
                model.CurrentNode = node;
                Information("Go to " + node.Value.Name);
                view.UpdateTree(model.CurrentNode);
                GetUrl();
            }
            else
            {
                // Download file
                view.OpenDialogDownload();
            }
        }

        /// <summary>
        /// Go to parent node
        /// </summary>
        public void GoToParent()
        {
            NodeModel parent = model.CurrentNode.Parent;
            if (parent != null)
            {
                model.CurrentNode = parent;
                view.UpdateTree(model.CurrentNode);
                GetUrl();
            }
        }

        /// <summary>
        /// Retrieve the current link
        /// </summary>
        /// <returns></returns>
        public String GetUrl()
        {
            NodeModel cur = model.CurrentNode;
            String url = "";
            while (cur != null)
            {
                url = cur.Value.Name + url;
                cur = cur.Parent;
            }
            view.Url = url;
            return url;
        }

        /// <summary>
        /// Go to the folder by uri
        /// </summary>
        /// <param name="url"></param>
        public void GoToUrl(String url)
        {
            String[] words = url.Split('/');
            NodeModel current, tmp;

            // Find the container
            if(words.Count() > 0)
            {
                FileModel file = model.Containers.FindFileModel(words[0] + "/");
                if (file != null)
                {
                    NodeModel root = GetTree(file);
                    current = root;

                    for (int j = 1; j < words.Count(); j++)
                    {
                        String nameWord = words[j];
                        if (nameWord != String.Empty)
                        {
                            Boolean isLastWord = (j == (words.Count() - 1));

                            if (!isLastWord)
                            {
                                nameWord += "/";
                            }
                            if ((tmp = current.FindNode(nameWord)) != null)
                            {
                                current = tmp;
                            }
                        }
                    }
                    model.Root = root;
                    model.CurrentNode = current;
                    view.UpdateTree(model.CurrentNode);
                    GetUrl();
                }
            }
        }

        /// <summary>
        /// Check if a specific child node is a directory or not
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Boolean IsNodeDirectory(int index)
        {
            return model.CurrentNode.Children.ElementAt(index).Value.IsDirectory;
        }

        /// <summary>
        /// Create folder
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public Boolean CreateFolder(String url, Boolean isFolder)
        {
            if (!isFolder) 
            {
                url = url.Replace("/", "");
                if (tools.ContainerExists(url))
                {
                    return false;
                }
            }

            if (url.ElementAt(url.Length - 1) != '/')
            {
                url += "/";
            }
            String newUrl = (isFolder ? GetUrl() : "");
            if (tools.CreateFolder(newUrl + url))
            {
                if (isFolder)
                {
                    NodeModel tmp = new NodeModel(new FileModel() { Name = url, IsDirectory = true, Uri = newUrl });
                    model.CurrentNode.AddChild(tmp);
                    view.UpdateTree(model.CurrentNode);
                    GetUrl();
                }
                else
                {
                    GetContainers();
                }
            }
            Information("Folder created successfully");
            return true;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        public void UploadFile(String file)
        {
            FileModel newFile = tools.UploadFile(GetUrl(), file);
            Information("File Uploaded successfully");
            model.CurrentNode.AddChild(newFile);
            view.UpdateTree(model.CurrentNode);
            GetUrl();
        }

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="index"></param>
        /// <param name="uriFolder"></param>
        public void Download(int index, String uriFolder)
        {
            NodeModel fileModel = model.CurrentNode.Children[index];
            ParseTree(fileModel, MethodNode.DOWNLOAD, "", uriFolder);
            Information("Download successfully");
        }

        /// <summary>
        /// Delete container
        /// </summary>
        public void DeleteContainer()
        {
            ParseTree(model.Root, MethodNode.DELETE, "");
            Information("Delete successfully");
            GetContainers();
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="index"></param>
        public void Delete(int index)
        {
            NodeModel fileModel = model.CurrentNode.Children[index];
            ParseTree(fileModel, MethodNode.DELETE, "");
            Information("File/Folder deleted successfully");
            fileModel.children.Clear();
            model.CurrentNode.RemoveChild(fileModel);
            view.UpdateTree(model.CurrentNode);
            GetUrl();
        }

        /// <summary>
        /// Parse tree for deleting or downloading
        /// </summary>
        /// <param name="node"></param>
        /// <param name="method"></param>
        /// <param name="uriFolderNetwork"></param>
        /// <param name="uriToStore"></param>
        public void ParseTree(NodeModel node, MethodNode method, String uriFolderNetwork, String uriToStore = "")
        {
            uriFolderNetwork += node.Value.Name;
            if (!node.Value.IsDirectory)
            {
                DoMethod(node, method, uriFolderNetwork, uriToStore);
            }
            else
            {
                foreach (NodeModel nodeTmp in node.Children)
                {
                    ParseTree(nodeTmp, method, uriFolderNetwork, uriToStore);
                }
                if (node.Value.Uri != null)
                {
                    DoMethod(node, method, uriFolderNetwork, uriToStore);
                }
            }
        }

        /// <summary>
        /// Delete or download file
        /// </summary>
        /// <param name="node"></param>
        /// <param name="method"></param>
        /// <param name="uriFolderNetwork"></param>
        /// <param name="uriToStore"></param>
        public void DoMethod(NodeModel node, MethodNode method, String uriFolderNetwork = "", String uriToStore = "")
        {
            switch (method)
            {
                case MethodNode.DOWNLOAD:
                    tools.Download(node.Value.Uri, uriToStore, uriFolderNetwork);
                    break;
                case MethodNode.DELETE:
                    tools.Delete(node.Value.Uri);
                    break;
            }
        }

        /// <summary>
        /// Add into the rich textbox a message
        /// </summary>
        /// <param name="message"></param>
        private void Information(String message)
        {
            String datetime = DateTime.Now.ToString("HH:mm:ss");
            view.Information = "[" + datetime + "] " + message + "\n";
        }

    }
}
