using PokemonReviewApp.Models;
namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        decimal GetPokemonRating(int pokeId);
        bool PokemonExists(int pokeId);
        ICollection<Pokemon> GetPokemons();
    }
}
