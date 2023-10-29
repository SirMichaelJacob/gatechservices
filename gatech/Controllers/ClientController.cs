using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gatech.Models;

namespace gatech.Controllers
{
    public class ClientController : Controller
    {
        private Context db = new Context();

        // GET: Clients
        public ActionResult Index()
        {
            
            List<Client> clients = GetClientsFromDatabase();
            return View(clients);
        }

        // GET: Clients/Details/{id}
        public ActionResult Details(int id)
        {
            Client client = GetClientFromDatabase(id);
            return View(client);
        }

        private List<Client> GetClientsFromDatabase()
        {
            // Fetch clients from the database or any other data source
            var clients = db.Clients.ToList();

            // Return a list of clients
            // ...
            return clients;
        }

        private Client GetClientFromDatabase(int id)
        {
            // Fetch client by ID from the database or any other data source
            // Return the client object
            // ...
            var client = db.Clients.Where(x=>x.ClientId==id).FirstOrDefault();
            return client;
        }

    }
}