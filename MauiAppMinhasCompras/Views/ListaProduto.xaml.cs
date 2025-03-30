using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    //Integração com interface gráfica com a listview
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();

        lst_produtos.ItemsSource = lista;
    }

    //Toda vez que a interface aparecer há uma busca no SQlite na lista de produtos e abastece na ObservableColeection
    protected async override void OnAppearing()
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		//Botão de navegação para adicionar produto
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());
		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        //Preenchimento da lista para o search sem acumulo de diversos itens da busca
        try
        {
            string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		//Botão para somar todos os itens da lista de produtos
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos",msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        //Botão para remover tanto da Observablelist quanto da listview
        try
        {
            MenuItem selecinado = sender as MenuItem;

            Produto p = selecinado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        //Botão para ver o produto selecionado da lista
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void picker_categoria_filtro_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_categoria_filtro.SelectedItem != null)
        {
            string categoriaSelecionada = picker_categoria_filtro.SelectedItem.ToString();
            FiltrarProdutosPorCategoria(categoriaSelecionada);
        }
    }

    private async void FiltrarProdutosPorCategoria(string categoria)
    {
        try
        {
            lista.Clear();
            List<Produto> produtos = await App.Db.GetAll();

            var produtosFiltrados = produtos
                .Where(p => p.Categoria != null && p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                .ToList();

            produtosFiltrados.ForEach(p => lista.Add(p));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao filtrar: {ex.Message}", "OK");
        }
    }

    private void ToolbarItem_Clicked_Relatorio(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.RelatorioGastos());
    }
}