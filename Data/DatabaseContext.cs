using Microsoft.Data.SqlClient;
using System;
using System.Data;
using GerenciadorTarefas.Models;

namespace GerenciadorTarefas.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext()
        {
            _connectionString = "Server=localhost;Database=GerenciadorTarefas;Trusted_Connection=True;TrustServerCertificate=True;";
            CriarTabelaSeNaoExistir();
        }

        private void CriarTabelaSeNaoExistir()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tarefas')
                    BEGIN
                        CREATE TABLE Tarefas (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Nome NVARCHAR(100) NOT NULL,
                            Data DATETIME NOT NULL,
                            Status NVARCHAR(50) NOT NULL
                        )
                    END", connection);
                command.ExecuteNonQuery();
            }
        }

        public DataTable ObterTarefas()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Tarefas", connection);
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public void InserirTarefa(Tarefa tarefa)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    INSERT INTO Tarefas (Nome, Data, Status)
                    VALUES (@Nome, @Data, @Status)", connection);

                command.Parameters.AddWithValue("@Nome", tarefa.Nome);
                command.Parameters.AddWithValue("@Data", tarefa.Data);
                command.Parameters.AddWithValue("@Status", tarefa.Status);

                command.ExecuteNonQuery();
            }
        }

        public void AtualizarTarefa(Tarefa tarefa)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    UPDATE Tarefas 
                    SET Nome = @Nome, Data = @Data, Status = @Status
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", tarefa.Id);
                command.Parameters.AddWithValue("@Nome", tarefa.Nome);
                command.Parameters.AddWithValue("@Data", tarefa.Data);
                command.Parameters.AddWithValue("@Status", tarefa.Status);

                command.ExecuteNonQuery();
            }
        }

        public void ExcluirTarefa(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Tarefas WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
} 