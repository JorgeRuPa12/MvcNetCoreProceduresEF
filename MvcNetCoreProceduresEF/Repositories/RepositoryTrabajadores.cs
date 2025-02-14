﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;
using System.Data;

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryTrabajadores
    {
        private HospitalContext context;
        public RepositoryTrabajadores(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<TrabajadoresModel> GetTrabajadoresModelAsync()
        {
            var consulta = from datos in this.context.Trabajadores
                           select datos;
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = await consulta.CountAsync();
            model.SumaSalarial = await consulta.SumAsync(z => z.Salario);
            model.MediaSalarial = (int) await consulta.AverageAsync(x => x.Salario);
            return model;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Trabajadores
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<TrabajadoresModel> GetTrabajadoresModelOficioAsync(string oficio)
        {
            string sql = "SP_WORKERS_OFICIO @oficio, @personas OUT, @media OUT, @suma OUT";
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamPersonas = new SqlParameter("@personas", -1);
            pamPersonas.Direction = ParameterDirection.Output;
            SqlParameter pamMedia = new SqlParameter("@media", -1);
            pamMedia.Direction = ParameterDirection.Output;
            SqlParameter pamSuma = new SqlParameter("@suma", -1);
            pamSuma.Direction = ParameterDirection.Output;

            var consulta = this.context.Trabajadores.FromSqlRaw(sql, pamOficio, pamPersonas, pamMedia, pamSuma);
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = int.Parse(pamPersonas.Value.ToString());
            model.MediaSalarial = int.Parse(pamMedia.Value.ToString());
            model.SumaSalarial = int.Parse(pamSuma.Value.ToString());
            return model;
        }
    }
}
