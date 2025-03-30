using System.Collections.ObjectModel;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	//Integração com interface gráfica com a listview
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produto.ItemsSource = lista;
	}

	//Toda vez que a interface aparecer há uma busca no SQlite na lista de produtos e abastece na ObservableColeection
    protected async override void OnAppearing()
    {
		List<Produto> tmp = await App.Db.GetAll();

		tmp.ForEach( i => lista.Add(i));
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
		string q = e.NewTextValue;

		lista.Clear();

		List<Produto> tmp = await App.Db.Search(q);

		tmp.ForEach ( i => lista.Add(i));
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		//Botão para somar todos os itens da lista de produtos
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos",msg, "OK");
    }

    private void MenuItem_Clicked(object sender, EventArgs e)
    {

    }
}