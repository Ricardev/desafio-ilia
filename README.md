# Ília - Desafio Técnico

## Para executar o projeto
É necessário que possua o Docker instalado na sua máquina. Com o docker rodando, execute o comando "sh run_docker.sh" na raiz do repositório, utilizando o *Git Bash*. Com isso o shellscript executará o "container run" e teremos o nosso banco de dados, nesse caso o Postgres. Resolvi não colocar migrations e fazer o padrão de projeto Code First para facilitar a vida dos senhores testadores a fim de não precisarem executar nenhuma migration, então estou enviando o arquivo init.sql para dentro do container do postgres com os comandos para criação das tabelas.
Após isso, é só subir o serviço usando o terminal, F5 ou qualquer outro método a sua escolha e ser feliz.
A página do Swagger abre automaticamente, é só seguir a documentação provida e poderá utilizar os endpoints.
