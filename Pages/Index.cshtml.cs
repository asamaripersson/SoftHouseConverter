using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace SoftHouseConverter.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            ModelState.Clear();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (file?.Length == 0 || file == null)
            {
                ModelState.AddModelError("file", "Please select a file.");
                return Page();
            }
            if (file.ContentType != "text/plain")
            {
                ModelState.AddModelError("file", "Only .txt files are allowed.");
                return Page();
            }

            try
            {
                // Parse the uploaded file
                List<Person> people = await ParseFileAsync(file);

                // Serialize to XML
                string xmlFilePath = "people.xml";
                await SerializeToXmlAsync(people, xmlFilePath);

                // XML file for download
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(xmlFilePath);
                return File(fileBytes, "application/xml", xmlFilePath);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        private async Task<List<Person>> ParseFileAsync(IFormFile file)
        {
            List<Person> people = new List<Person>();
            Person? currentPerson = null;
            Family? currentFamilyMember = null;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var line = "";
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    line = line.TrimEnd('\\');

                    string[] parts = line.Split('|');
                    if (parts.Length == 0)
                        continue;

                    switch (parts[0])
                    {
                        case "P":
                            currentPerson = new Person
                            {
                                FirstName = parts.Length > 1 ? parts[1] : "",
                                LastName = parts.Length > 2 ? parts[2] : ""
                            };
                            //Reset familymember for each new person
                            currentFamilyMember = null;

                            people.Add(currentPerson);
                            break;
                        case "T":
                            if (currentPerson != null)
                            {
                                currentPerson.Phone ??= new Phone
                                {
                                    Mobile = parts.Length > 1 ? parts[1] : null,
                                    LandLine = parts.Length > 2 ? parts[2] : null
                                };
                            }
                            if (currentFamilyMember != null)
                            {
                                currentFamilyMember.Phone ??= new Phone
                                {
                                    Mobile = parts.Length > 1 ? parts[1] : null,
                                    LandLine = parts.Length > 2 ? parts[2] : null
                                };
                            }
                            break;
                        case "A":
                            if (currentPerson != null)
                            {
                                currentPerson.Address ??= new Address
                                {
                                    Street = parts.Length > 1 ? parts[1] : null,
                                    City = parts.Length > 2 ? parts[2] : null,
                                    Zip = parts.Length > 3 ? parts[3] : null
                                };
                            }
                            if (currentFamilyMember != null)
                            {
                                currentFamilyMember.Address ??= new Address
                                {
                                    Street = parts.Length > 1 ? parts[1] : null,
                                    City = parts.Length > 2 ? parts[2] : null,
                                    Zip = parts.Length > 3 ? parts[3] : null
                                };
                            }
                            break;
                        case "F":
                            if (currentPerson != null)
                            {
                                currentPerson.Family ??= new List<Family>();

                                currentFamilyMember = new Family
                                {
                                    Name = parts.Length > 1 ? parts[1] : "",
                                    Born = parts.Length > 2 ? int.TryParse(parts[2], out int birthYear) ? birthYear : 0 : 0
                                };
                                currentPerson.Family.Add(currentFamilyMember);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return people;
        }

        private async Task SerializeToXmlAsync(List<Person> persons, string filePath)
        {
            var people = new People { Persons = persons };
            XmlSerializer serializer = new XmlSerializer(typeof(People));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                {
                    await Task.Run(() => serializer.Serialize(writer, people));
                }
            }
        }
    }
}
