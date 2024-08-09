using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public List<Roommate> GetAllRoommates()
        {

            using (SqlConnection conn = Connection)
            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, RentPortion, MoveInDate
                                        FROM Roommate";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int idValue = reader.GetInt32(reader.GetOrdinal("Id"));

                        string firstNameValue = reader.GetString(reader.GetOrdinal("FirstName"));

                        string lastNameValue = reader.GetString(reader.GetOrdinal("LastName"));

                        int rentPortionValue = reader.GetInt32(reader.GetOrdinal("RentPortion"));

                        DateTime moveInDateValue = reader.GetDateTime(reader.GetOrdinal("MoveInDate"));

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            RentPortion = rentPortionValue,
                            MovedInDate = moveInDateValue,
                            Room = null
                        };

                        roommates.Add(roommate);
                    }
                    reader.Close();

                    return roommates;
                }
            }
        }

        public Roommate GetRoommateById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //output their first name, their rent portion, and the name of the room they occupy.
                    cmd.CommandText = @"SELECT FirstName, RentPortion, Room.Name AS RoomName
                                        FROM Roommate
                                        LEFT JOIN Room ON Room.Id = Roommate.RoomId
                                        WHERE Roommate.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    if (reader.Read())
                    {
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            Room = new Room
                            {
                                Name = reader.GetString(reader.GetOrdinal("RoomName"))
                            }
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }
    }
}
