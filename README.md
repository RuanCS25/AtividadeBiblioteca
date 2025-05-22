# BiblioTech

Projeto de biblioteca online desenvolvido em sala de aula, utilizando ASP.NET Core MVC e boas práticas de desenvolvimento.

## ✨ Funcionalidades

- Estrutura de projeto em ASP.NET Core MVC
- Acesso a dados com Entity Framework Core
- Autenticação e autorização com ASP.NET Core Identity
- Registro, login e controle de acesso por perfil de usuário (Admin e Aluno)
- Cadastro e gerenciamento de:
  - Livros
  - Gêneros Literários
  - Usuários
- Movimentação de livros (empréstimo e devolução)
- Reserva de livros

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **ASP.NET Core Identity**
- **Bootstrap 5.3**
- **SQL Server (LocalDB)**

## 🚀 Como Executar o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 ou VS Code
- SQL Server Express LocalDB

### Passos

1. Clone este repositório:
   ```bash
   git clone https://github.com/ProfCristianoDePaula/Biblioteca.git
   ```
2. Acesse a pasta do projeto:
   ```bash
   cd BiblioTech
   ```
3. Restaure os pacotes NuGet:
   ```bash
   dotnet restore
   ```
4. Atualize a string de conexão no arquivo `appsettings.json` (se necessário):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=localhost\\SQLSENAI; Initial Catalog = Biblioteca;Integrated Security = True;TrustServerCertificate = True"
   }
   ```
5. Aplique as migrações e crie o banco de dados:
   ```bash
   Update-Database
   ```


## 📖 Uso

- **Admin**: pode cadastrar livros, gêneros e gerenciar usuários e movimentações.
- **Aluno**: pode visualizar o catálogo e realizar reservas de livros.


## 📄 Licença

Este projeto está licenciado sob a MIT License. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
