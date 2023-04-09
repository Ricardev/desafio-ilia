create schema Controle_Ponto;


create table Controle_Ponto.Registro_Ponto (
    id SERIAL,
    dia_hora TIMESTAMP not null
);

create index IX_RegistroPonto_DiaHora on Controle_Ponto.Registro_Ponto (dia_hora);