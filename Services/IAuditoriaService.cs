using madame_moka.Models;

namespace madame_moka.Services
{
	public interface IAuditoriaService
	{
		void RegistrarMovimiento(int idUsuario, int movimiento, string descripcion);
		List<Auditoria> ObtenerAuditorias();
	}
}
