using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class RelatorioGastos : ContentPage
{
    ObservableCollection<GastoPorCategoria> listaGastos = new ObservableCollection<GastoPorCategoria>();

    public RelatorioGastos()
    {
        InitializeComponent();
        lst_gastos.ItemsSource = listaGastos;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CarregarRelatorio();
    }

    private async Task CarregarRelatorio()
    {
        try
        {
            listaGastos.Clear();

            List<Produto> produtos = await App.Db.GetAll();

            var gastosPorCategoria = produtos
                .GroupBy(p => p.Categoria)
                .Select(g => new GastoPorCategoria
                {
                    Categoria = g.Key,
                    TotalGasto = g.Sum(p => p.Total)
                })
                .ToList();

            gastosPorCategoria.ForEach(g => listaGastos.Add(g));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}

// Classe auxiliar para armazenar os gastos por categoria
public class GastoPorCategoria
{
    public string Categoria { get; set; }
    public double TotalGasto { get; set; }
}