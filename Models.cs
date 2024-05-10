using System.Xml.Serialization;

public class PeopleClass
{
    [XmlElement("people")]
    public List<Person>? People { get; set; }
}
public class Person
{
    [XmlElement("firstName")]
    public required string FirstName { get; set; }

    [XmlElement("lastName")]
    public required string LastName { get; set; }

    [XmlElement("address")]
    public Address? Address { get; set; }

    [XmlElement("phone")]
    public Phone? Phone { get; set; }

    [XmlElement("family")]
    public List<Family>? Family { get; set; }
}

public class Family
{
    [XmlElement("name")]
    public required string Name { get; set; }

    [XmlElement("born")]
    public int Born { get; set; }

    [XmlElement("address")]
    public Address? Address { get; set; }

    [XmlElement("phone")]
    public Phone? Phone { get; set; }
}

public class Address
{
    [XmlElement("street")]
    public string? Street { get; set; }

    [XmlElement("city")]
    public string? City { get; set; }

    [XmlElement("zip")]
    public string? Zip { get; set; }

}

public class Phone
{
    [XmlElement("mobile")]
    public string? Mobile { get; set; }
    [XmlElement("landLine")]
    public string? LandLine { get; set; }
}