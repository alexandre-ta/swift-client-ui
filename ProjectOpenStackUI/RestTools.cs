using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// Represents the tools used to access, delete, ...
    /// </summary>
    class RestTools
    {
        /// <summary>
        /// Authentication URI
        /// </summary>
        private String auth_url;

        /// <summary>
        /// Authentication version
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
        /// Token ID
        /// </summary>
        private String token_id;

        /// <summary>
        /// Tenant ID
        /// </summary>
        private String tenant_id;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth_url"></param>
        /// <param name="auth_version"></param>
        /// <param name="storage_url"></param>
        /// <param name="storage_version"></param>
        public RestTools(String auth_url, String auth_version, String storage_url, String storage_version)
        {
            this.auth_url = auth_url;
            this.auth_version = auth_version;
            this.storage_url = storage_url;
            this.storage_version = storage_version;
        }

        /// <summary>
        /// Check if the user can connect or not
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Boolean Connect(String username, String password)
        {
            RestClient rc = new RestClient(auth_url);
            RestRequest request = new RestRequest(auth_version + "/", Method.GET);
            request.AddHeader("X-Auth-User", username);
            request.AddHeader("X-Auth-Key", password);
            IRestResponse response = rc.Execute(request);
            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Get list of tokens
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Boolean GetTokens(String tenant, String username, String password)
        {
            System.Net.WebRequest request = WebRequest.Create(auth_url + auth_version + "/tokens");
            HttpWebResponse response = null;

            request.ContentType = "application/json";
            request.Method = "POST";
            byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes("{\"auth\": {\"tenantName\":\""+tenant+"\", \"passwordCredentials\":{\"username\":\""+username+"\", \"password\":\""+password+"\"}}}");


            Stream reqstr = request.GetRequestStream();
            reqstr.Write(buffer, 0, buffer.Length);
            reqstr.Close();

            // Get response  
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
            // Get the response stream  
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string results = reader.ReadToEnd();

            JObject jObject = JObject.Parse(results);
            token_id = jObject["access"]["token"]["id"].ToString();
            tenant_id = jObject["access"]["token"]["tenant"]["id"].ToString();

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Get list of files
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public FilesModel GetFiles(String dir)
        {
            FilesModel files = new FilesModel();
            int size = 0, count = 0;
            Boolean isDirectory = false;
            String name = null;
            String hash = null;
            String last_modified = null;
            String content_type = null;

            RestClient rc = new RestClient(storage_url);
            RestRequest request = new RestRequest(storage_version + "/AUTH_{tenant}/{dir}?format=json", Method.GET);
            request.AddUrlSegment("tenant", tenant_id);
            request.AddUrlSegment("dir", dir);
            request.AddHeader("X-Auth-Token", token_id);
            
            IRestResponse response = rc.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            // Hack the string in order to parse with json tool
            String tmp = "{\"results\":" + response.Content + "}";
            // Parse JSON into dynamic object, convenient!
            JObject results = JObject.Parse(tmp);
            // Process each file
            foreach (var result in results["results"])
            {
                isDirectory = (result.Count() < 4);
                size = (result["bytes"] != null) ? (int)result["bytes"] : 0;
                name = (result["name"] != null) ? (String)result["name"] : null;
                hash = (result["hash"] != null) ? (String)result["hash"] : null;
                last_modified = (result["last_modified"] != null) ? (String)result["last_modified"] : null;
                content_type = (result["content_type"] != null) ? (String)result["content_type"] : null;
                count = (result["count"] != null) ? (int)result["count"] : 0;

                files.AddFile(new FileModel()
                {
                    IsDirectory = isDirectory,
                    Size = size,
                    Name = name,
                    Hash = hash,
                    Last_modified = last_modified,
                    Content_type = content_type,
                    Count = count
                });
            }
            return files;
        }

        /// <summary>
        /// Create folder or container
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Boolean CreateFolder(String url)
        {
            RestClient rc = new RestClient(storage_url);
            IRestRequest request = new RestRequest("/{version}/AUTH_{tenant}/{newdir}", Method.PUT);
            request.AddUrlSegment("version", storage_version);
            request.AddUrlSegment("tenant", tenant_id);
            request.AddUrlSegment("newdir", url);
            request.AddHeader("X-Auth-Token", token_id);
            IRestResponse response = rc.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return false;
            }
            else
            {
                return true;
            } 
        }

        /// <summary>
        /// Upload file into database
        /// </summary>
        /// <param name="container"></param>
        /// <param name="uriFile"></param>
        /// <returns></returns>
        public FileModel UploadFile(String container, String uriFile)
        {
            FileModel modelToSend = null;
            String fullPath = container + Path.GetFileName(uriFile);
            String storageLink = storage_url + storage_version + "/AUTH_" + tenant_id + "/" + fullPath;

            StringBuilder requestUriFile = new StringBuilder(storageLink);

            byte[] arr = System.IO.File.ReadAllBytes(uriFile);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUriFile.ToString());
            request.Method = "PUT";
            request.ContentType = "text/plain";
            request.ContentLength = arr.Length;
            request.Headers.Add("X-Auth-Token", token_id);

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(arr, 0, arr.Length);
            dataStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                modelToSend = new FileModel()
                {
                    Name = Path.GetFileName(uriFile),
                    Uri = fullPath,
                    Size = arr.Length,
                    Last_modified = response.LastModified.ToString(),
                    IsDirectory = false,
                    Hash = response.GetHashCode().ToString(),
                    Content_type = response.ContentType
                };
            }
            return modelToSend;
        }

        /// <summary>
        /// Download file from database
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="localToStore"></param>
        /// <param name="fileToStore"></param>
        /// <returns></returns>
        public Boolean Download(String uri, String localToStore, String fileToStore)
        {
            if (uri.ElementAt(uri.Length - 1) != '/')
            {
                RestClient rc = new RestClient(storage_url);
                IRestRequest request = new RestRequest("/v1/AUTH_{tenant}/{uri}", Method.GET);
                request.AddUrlSegment("tenant", tenant_id);
                request.AddUrlSegment("uri", uri);
                request.AddHeader("X-Auth-Token", token_id);
                IRestResponse response = rc.Execute(request);

                String fileName = Path.Combine(localToStore, fileToStore.Replace("/", "\\"));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FileInfo fi = new FileInfo(fileName);
                    if (!Directory.Exists(fi.DirectoryName))
                        Directory.CreateDirectory(fi.DirectoryName);
                    File.WriteAllBytes(fi.FullName, response.RawBytes);
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Boolean Delete(String uri)
        {
            RestClient rc = new RestClient(storage_url);
            IRestRequest request = new RestRequest("/{version}/AUTH_{tenant}/{uri}", Method.DELETE);
            request.AddUrlSegment("version", storage_version);
            request.AddUrlSegment("tenant", tenant_id);
            request.AddUrlSegment("uri", uri);
            request.AddHeader("X-Auth-Token", token_id);

            IRestResponse response = rc.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if a container exists
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public Boolean ContainerExists(String containerName)
        {
            RestClient rc = new RestClient(storage_url);
            IRestRequest request = new RestRequest("/{version}/AUTH_{tenant}/{container}", Method.HEAD);
            request.AddUrlSegment("version", storage_version);
            request.AddUrlSegment("tenant", tenant_id);
            request.AddUrlSegment("container", containerName);
            request.AddHeader("X-Auth-Token", token_id);

            IRestResponse response = rc.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK ||
                response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}
