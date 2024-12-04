using System.ComponentModel;

namespace Chrono.Console.Enums;

public enum EnumTimeViewsOptions
{
    [Description("Listar todos")]
    ListarTodos,
    [Description("Buscar por código")]
    BuscarPorCodigo,
    [Description("Buscar por atividade")]
    BuscarPorAtividade,
    [Description("Buscar por dia")]
    BuscarPorDia,
    [Description("Buscar por mês")]
    BuscarPorMes,
    [Description("Sair")]
    Sair
}