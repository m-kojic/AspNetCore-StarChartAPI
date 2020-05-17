using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();

            if (celestialObject.Satellites == null)
                celestialObject.Satellites = new List<Models.CelestialObject>();

            celestialObject.Satellites.Add(celestialObject);
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (celestialObjects == null || celestialObjects.Count == 0)
                return NotFound();

            foreach (var item in celestialObjects)
            {
                if (item.Satellites == null)
                    item.Satellites = new List<Models.CelestialObject>();

                item.Satellites.AddRange(celestialObjects);
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var item in celestialObjects)
            {
                if (item.Satellites == null)
                    item.Satellites = new List<Models.CelestialObject>();

                item.Satellites.Add(item);
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var currentCelestialObject = _context.CelestialObjects.Find(id);
            if (currentCelestialObject == null)
                return NotFound();

            currentCelestialObject.Name = celestialObject.Name;
            currentCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            currentCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(currentCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var currentCelestialObject = _context.CelestialObjects.Find(id);
            if (currentCelestialObject == null)
                return NotFound();

            currentCelestialObject.Name = name;

            _context.CelestialObjects.Update(currentCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            var objectsToDelete = new List<CelestialObject>();

            foreach (var item in celestialObjects)
            {
                if (item.Id == id || item.OrbitedObjectId == id)
                    objectsToDelete.Add(item);
            }

            if (objectsToDelete.Count == 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(objectsToDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
