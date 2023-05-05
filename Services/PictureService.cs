using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;

class PictureService
{
    public List<Show>? Planets;

    public PictureService()
    {
        Read();
    }

    public void Read()
    {
        if(File.Exists("baza.json"))
        {
            var satr = File.ReadAllText("baza.json");
            Planets = (JsonConvert.DeserializeObject<List<Show>>(satr) ?? new List<Show>());
        }
        else
        {
            Planets = new List<Show>();
        }
    }
}