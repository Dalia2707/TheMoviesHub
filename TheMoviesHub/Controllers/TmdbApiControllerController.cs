using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TheMoviesHub.Class;
using TheMoviesHub.Models;

namespace TheMoviesHub.Controllers
{
    public class TmdbApiControllerController : Controller
    {
        // GET: TmdbApiController
        public ActionResult Index(string peopleName, int? page)
        {
            if (page != null)
                CallAPI(peopleName, Convert.ToInt32(page));
            Models.TheMoviesDb theMoviesDb = new Models.TheMoviesDb();
            theMoviesDb.searchText= peopleName;
            return View(theMoviesDb);
        }
        [HttpPost]
        public ActionResult Index(Models.TheMoviesDb theMoviesDb, string searchText)
        {
            if(ModelState.IsValid)
            {
                CallAPI(searchText, 0);
            }
            return View(theMoviesDb);
        }
        public void CallAPI(string seachText, int page)
        {
            int pageNo = Convert.ToInt32(page) == 0 ? 1 : Convert.ToInt32(page);

            //Calling API http://developers.themoviedb.org/3/search/search-people
            string apikey = "";
            HttpWebRequest apiRequest = WebRequest.Create("https://api.themoviedb.org/3/search/person?api_key=" + apikey + "&language=en-US&query=" + searchText + "&page=" + pageNo + "&include_adult=false") as HttpWebRequest;

            string apiResponse = "";
            ServicePointManager.SecurityProtocol= SecurityProtocolType.Ssl3
                | SecurityProtocolType.Tls
                |SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader= new StreamReader(response.GetResponseStream());
                apiResponse= reader.ReadToEnd();
            }
            //fin

            /*http://json2csharp.com*/
            ResponseSearchPeople rootObject = JsonConvert.DeserializeObject<ResponseSearchPeople>(apiResponse);

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"resultDiv\"><p>Names</p>");
            foreach (Result result in rootObject.results)
            {
                string image = result.profile_path == null ? Url.Content("~/Content/image/no-image.png") : "https://image.tmdb.org/t/p/w500/" + result.profile_path;
                string link = Url.Action("GetPerson", "TmdbApi", new {id = result.id});
                sb.Append("<div class=\"result\" resourceId=\"" + result.id + "\">" + link + "\"><img src=\"" + image + "\" />" + "<p>" + result.name + "</a></p></div>");
            }
            ViewBag.Result = sb.ToString();

            int pageSize = 20;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = pageNo;
            pagingInfo.TotalItem = rootObject.total_results;
            pagingInfo.ItemPerPage = pageSize;
            ViewBag.Paging = pagingInfo;
        }

        public ActionResult GetPerson(int id)
        {
            /*Calling API https://developers.themoviedb.org/3/people */
            string apikey = "";
            HttpWebRequest apiRequest = WebRequest.Create("https://api.themoviedb.org/3/person/" + id + "?api_key=" + apikey + "&language=en-US&query=") as HttpWebRequest;

            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse= reader.ReadToEnd();
            }
            /*fin*/

            /*http://json2csharp.com*/
            ResponsePerson rootObject = JsonConvert.DeserializeObject<ResponsePerson>(apiResponse);
            TheMoviesDb theMoviesDb = new TheMoviesDb();
            theMoviesDb.name = rootObject.name;
            theMoviesDb.biography = rootObject.biography;
            theMoviesDb.birthday=rootObject.birthday;
            theMoviesDb.place_of_birth=rootObject.place_of_birth;
            theMoviesDb.profile_path = rootObject.profile_path == null ? Url.Content("~/Content/Image/no-image.png") : "https://image.tmdb.org/org/t/p/w500/" + rootObject.profile_path;
            theMoviesDb.also_known_as = string.Join(",",rootObject.also_known_as);

            return View(theMoviesDb);
        }
    }
}
