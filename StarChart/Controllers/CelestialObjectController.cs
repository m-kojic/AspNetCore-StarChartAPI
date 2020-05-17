using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
