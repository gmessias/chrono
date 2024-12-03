using System.ComponentModel;

namespace Chrono.Console.Enums;

public enum EnumActivityViewsOptions
{
    [Description("Listar todos")]
    ListarTodos,
    [Description("Listar todos ativos")]
    ListarTodosAtivos,
    [Description("Listar por filtro")]
    ListarPorFiltro,
    [Description("Buscar por código")]
    BuscarPorCodigo,
    [Description("Buscar por nome")]
    BuscarPorNome,
    [Description("Sair")]
    Sair
}