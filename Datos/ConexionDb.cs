using MySql.Data.MySqlClient;

namespace SistemaRRHH.Datos
{
	public class ConexionDb
	{
		private readonly string _cadenaConexion;

		// Inyectamos la configuración para leer el appsettings.json
		public ConexionDb(IConfiguration configuracion)
		{
			_cadenaConexion = configuracion.GetConnectionString("ConexionMySql")!;
		}

		// Método que llamaremos cada vez que queramos hablar con MySQL
		public MySqlConnection ObtenerConexion()
		{
			return new MySqlConnection(_cadenaConexion);
		}
	}
}