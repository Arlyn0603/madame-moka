using Microsoft.Data.SqlClient;
using madame_moka.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace madame_moka.Services
{
	public class AuditoriaService : IAuditoriaService
	{
		private readonly IConfiguration _configuration;

		public AuditoriaService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void RegistrarMovimiento(int idUsuario, int movimiento, string descripcion)
		{
			try
			{
				Console.WriteLine($"Registrando auditoría: {idUsuario}, {movimiento}, {descripcion}");

				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"INSERT INTO Auditoria (IdUsuario, Movimiento, Descripcion, FechaHora) 
                             VALUES (@IdUsuario, @Movimiento, @Descripcion, @FechaHora)";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
						cmd.Parameters.AddWithValue("@Movimiento", movimiento);
						cmd.Parameters.AddWithValue("@Descripcion", descripcion);
						cmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);

						int rowsAffected = cmd.ExecuteNonQuery();
						Console.WriteLine($"Filas afectadas: {rowsAffected}");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error en auditoría: {ex.Message}");
			}
		}


		public List<Auditoria> ObtenerAuditorias()
		{
			List<Auditoria> auditorias = new List<Auditoria>();

			try
			{
				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"SELECT a.*, u.correo AS Usuario 
                                     FROM Auditoria a
                                     INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
                                     ORDER BY a.FechaHora DESC";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								auditorias.Add(new Auditoria
								{
									IdAuditoria = reader.GetInt32(reader.GetOrdinal("IdAuditoria")),
									IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
									Movimiento = reader.GetInt32(reader.GetOrdinal("Movimiento")),
									Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
									FechaHora = reader.GetDateTime(reader.GetOrdinal("FechaHora")),
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error obteniendo auditorías: {ex.Message}");
			}

			return auditorias;
		}
	}
}
