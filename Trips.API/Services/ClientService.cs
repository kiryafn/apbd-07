using Trips.API.Contracts.Requests;
using Trips.API.Entities;
using Trips.API.Exceptions;
using Trips.API.Repositories.Abstractions;
using Trips.API.Services.Abstractions;

namespace Trips.API.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly ITripRepository _tripRepository;

    public ClientService(IClientRepository clientRepository, ITripRepository tripRepository)
    {
        _clientRepository = clientRepository;
        _tripRepository = tripRepository;
    }

    public async Task<ICollection<ClientTrip>?> GetAllClientTripsAsync(int clientId, CancellationToken token = default)
        => await _clientRepository.GetClientTripsAsync(clientId, token);

    public async Task<int> CreateClientAsync(CreateClientRequest client, CancellationToken token = default)
    {
        var clientExists = await _clientRepository.ClientExistsByPeselAsync(client.Pesel, token);
        if (clientExists)
            throw new ClientWithPeselNumberExistsException(client.Pesel);

        var newClient = new Client
        {
            Telephone = client.Telephone,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Pesel = client.Pesel
        };
        
        var result = await _clientRepository.CreateClientAsync(newClient, token);
        return result.Id;
    }

    public async Task<bool> DeleteTourByIdAsync(int userId, int tripId, CancellationToken token = default)
    {
       return await _tripRepository.DeleteClientTripAsync(userId, tripId, token);
    }
    
}