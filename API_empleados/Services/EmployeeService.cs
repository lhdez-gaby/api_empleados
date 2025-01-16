using API_empleados.Models;
using Microsoft.Extensions.Options;
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

        public async Task<List<Employee>> GetEmployees()=>
        await _employees.Find(employee => true).ToListAsync();

        public async Task<Employee> GetEmployeeById(string id)=>
        await _employees.Find(employee=>employee.Id == id).FirstOrDefaultAsync();

        public async Task<Employee> CreateEmployee(Employee employee)
        {
            await _employees.InsertOneAsync(employee);
            return employee;
        }
    }
}