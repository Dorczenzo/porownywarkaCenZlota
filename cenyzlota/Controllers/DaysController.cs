using cenyzlota.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace cenyzlota.Controllers
{
    public class DaysController : Controller
    {
        string link;
        decimal priceMin = 100000;
        decimal priceMax = 0;
        DateTime dateMin;
        DateTime dateMax;
        List<decimal> priceList;


        ObservableCollection<Day> XmlToList(string start, string end, string cMin, string cMax)
        {
            priceList = new List<decimal>();

            link = ("http://api.nbp.pl/api/cenyzlota/" + start + "/" + end + "?format=xml");

            ObservableCollection<Day> daysList = new ObservableCollection<Day>();

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(link);

                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes(" / ArrayOfCenaZlota/CenaZlota");

                foreach (XmlNode node in nodes)
                {
                    string goldDate = node["Data"].InnerText;
                    string goldPrice = node["Cena"].InnerText.Replace('.', ',');
                    decimal tryPrice = Convert.ToDecimal(goldPrice);
                    priceList.Add(tryPrice);

                    try
                    {

                        if ((cMin == null || cMin == "" || tryPrice >= Convert.ToDecimal(cMin)) && (cMax == null || cMax == "" || tryPrice <= Convert.ToDecimal(cMax)))
                        {
                            daysList.Add(new Day
                            {
                                Date = goldDate,
                                Price = tryPrice
                            });
                        }

                    }
                    catch
                    {
                        ViewBag.error = "Zły format ceny.";
                    }

                    if (Convert.ToDecimal(goldPrice) < priceMin)
                    {
                        priceMin = Convert.ToDecimal(goldPrice);
                        dateMin = Convert.ToDateTime(goldDate);
                    }

                    if (Convert.ToDecimal(goldPrice) > priceMax)
                    {
                        priceMax = Convert.ToDecimal(goldPrice);
                        dateMax = Convert.ToDateTime(goldDate);
                    }
                }

                ViewBag.minPrice = "Najniższa cena (" + priceMin + ") w dniu " + dateMin.Day + "." + dateMin.Month + "." + dateMin.Year + ".";
                ViewBag.maxPrice = "Najwyższa cena (" + priceMax + ") w dniu " + dateMax.Day + "." + dateMax.Month + "." + dateMax.Year + ".";
                ViewBag.median = "Mediana z danego okresu wynosi " + Median(priceList) + ".";

            }
            catch
            {
                ViewBag.error = "Błąd w pobieraniu danych z serweru banku. Serwer niedostępny lub nieprawidłowe daty.";
            }


            return (daysList);

        }

        private decimal Median(List<decimal> list)
        {
            list.Sort();
            int count = list.Count;

            if (count % 2 == 0)
            {
                decimal a = list[count / 2 - 1];
                decimal b = list[count / 2];
                return Math.Round((decimal)((a + b) / 2), 2);
            }
            else
            {
                return list[count / 2];
            }

        }

        // GET: Days
        public ActionResult Index(string DateStart, string DateEnd, string CenaMin, string CenaMax)
        {
            return View(XmlToList(DateStart, DateEnd, CenaMin, CenaMax));
        }
    }
}