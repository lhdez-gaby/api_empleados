using MongoDB.Bson.Serialization.Attributes;

namespace API_empleados.Models
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {get;set;}
        public string Nombre {get;set;} = null!;
        public string ApellidoMaterno {get;set;} = null!;
        public string ApellidoPaterno {get;set;} = null!;
        public string FechaNacimiento {get;set;} = null!;
        public string FechaContratacion {get;set;} = null!;
        public string Cargo {get;set;} = null!;
        public string Departamento {get;set;} = null!;
        public int Salario {get;set;}
    }
}