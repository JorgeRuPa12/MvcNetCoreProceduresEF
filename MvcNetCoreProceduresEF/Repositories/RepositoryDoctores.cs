using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;
using System.Data.Common;

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryDoctores
    {
        private EnfermoContext context;
        public RepositoryDoctores(EnfermoContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> GetEspecialidadesAsync()
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ESPECIALIDADES";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<string> especialidades = new List<string>();
                while (await reader.ReadAsync())
                {
                    string especialidad = reader["ESPECIALIDAD"].ToString();
                    especialidades.Add(especialidad);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return especialidades;
            }
        }

        public async Task<List<Doctor>> GetDoctoresEspecialidad(string especialidad, int salario)
        {
            string sql = "SP_DOCTORES_ESPECIALIDAD @especialidad, @salario";

            SqlParameter pamInscripcion = new SqlParameter("@especialidad", especialidad);
            SqlParameter pamSalario = new SqlParameter("@salario", salario);
            var consulta = await this.context.Doctores.FromSqlRaw(sql, pamInscripcion, pamSalario).ToListAsync();
            return consulta;
        }
    }
}
