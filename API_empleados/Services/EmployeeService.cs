using API_empleados.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API_empleados.Services
{
    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;
        public EmployeeService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _employees = mongoDatabase.GetCollection<Employee>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Employee>> GetEmployees() =>
        await _employees.Find(employee => true).ToListAsync();

        public async Task<Employee> GetEmployeeById(string id) =>
        await _employees.Find(employee => employee.Id == id).FirstOrDefaultAsync();

        public async Task<object> GetEmployeesBySearch(
            string? nombre,
            string? cargo,
            string? departamento,
            string? fechaInicial,
            string? fechaFinal,
            int pagina,
            int porPagina
        )
        {
            if (pagina <= 0) pagina = 1;
            if (porPagina <= 0) porPagina = 5;

            int skip = (pagina - 1) * porPagina;

            var filterBuilder = Builders<Employee>.Filter;
            var filters = new List<FilterDefinition<Employee>>();

            if (!string.IsNullOrEmpty(nombre))
                filters.Add(filterBuilder.Regex("Nombre", new BsonRegularExpression(nombre, "i"))); // Búsqueda por nombre (insensible a mayúsculas)
            if (!string.IsNullOrEmpty(cargo))
            {
                var listaCargo = cargo.Split('.');
                filters.Add(filterBuilder.In("Cargo", listaCargo));
            }

            if (!string.IsNullOrEmpty(departamento))
            {
                var listaDepartamento = departamento.Split('.');
                filters.Add(filterBuilder.In("Departamento", listaDepartamento));
            }
            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                filters.Add(filterBuilder.Gte("FechaContratacion", fechaInicial)); // Mayor o igual a la fecha de inicio
                filters.Add(filterBuilder.Lte("FechaContratacion", fechaFinal));   // Menor o igual a la fecha de fin
            }

            var finalFilter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            // Contar el total de empleados que cumplen los filtros (sin paginación)
            var totalFilteredEmployees = await _employees.CountDocumentsAsync(finalFilter);

            // Realizamos la consulta a MongoDB con los filtros y la paginación
            var employees = await _employees
                .Find(finalFilter)
                .Skip(skip)
                .Limit(porPagina)
                .ToListAsync();

            return new 
            {
                TotalCount = totalFilteredEmployees, // Total filtrado
                Employees = employees // Lista paginada
            };

        }


        public async Task<Employee> CreateEmployee(Employee employee)
        {
            await _employees.InsertOneAsync(employee);
            return employee;
        }
    }
}