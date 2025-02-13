using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;
using System.Data.Common;

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryEnfermos
    {
        private EnfermoContext context;
        public RepositoryEnfermos(EnfermoContext context)
        {
            this.context = context;
        }

        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            using (DbCommand com =this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_TODOS_ENFERMOS";
                com.CommandType = System.Data.CommandType.Text;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Enfermo> enfermos = new List<Enfermo>();
                while (await reader.ReadAsync())
                {
                    Enfermo enfermo = new Enfermo
                    {
                        Inscripcion = reader["INSCRIPCION"].ToString(),
                        Apellido = reader["APELLIDO"].ToString(),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Genero = reader["S"].ToString(),
                    };
                    enfermos.Add(enfermo);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();

                return enfermos;
            }
        }

        public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
        {
            string sql = "SP_FIND_ENFERMO @INSCRIPCION";

            SqlParameter pamInscripcion = new SqlParameter("@INSCRIPCION", inscripcion);
            var consulta = await this.context.Enfermos.FromSqlRaw(sql, pamInscripcion).ToListAsync();

            return consulta.FirstOrDefault();
        }

        public void DeleteEnfermo(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";

            SqlParameter pamInscripcion = new SqlParameter("@INSCRIPCION", inscripcion);

            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamInscripcion);
                com.Connection.Open();
                com.ExecuteNonQuery();
                com.Connection.Close();
                com.Parameters.Clear();
            }
        }

        public async Task DeleteEnfermoRaw(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @INSCRIPCION"; 
            SqlParameter pamInscripcion = new SqlParameter("@INSCRIPCION", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamInscripcion);

       
        }

        public async Task InsertEnfermoAsync(Enfermo enfermo, string nss)
        {
            string sql = "SP_INSERT_ENFERMO @APELLIDO, @DIRECCION, @FECHA, @S, @NSS";
            SqlParameter pamApellido = new SqlParameter("@APELLIDO", enfermo.Apellido);
            SqlParameter pamDireccion = new SqlParameter("@DIRECCION", enfermo.Direccion);
            SqlParameter pamFecha = new SqlParameter("@FECHA", enfermo.FechaNacimiento);
            SqlParameter pamGenero = new SqlParameter("@S", enfermo.Genero);
            SqlParameter pamNss = new SqlParameter("@NSS", nss);

            await this.context.Database.ExecuteSqlRawAsync(sql, pamApellido, pamDireccion, pamFecha, pamGenero, pamNss);
        }
    }
}
