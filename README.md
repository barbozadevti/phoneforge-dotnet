# Desafio POO em .NET — Smartphones

Projeto do desafio da trilha .NET da [DIO](https://www.dio.me/), com base no repositório [trilha-net-poo-desafio](https://github.com/digitalinnovationone/trilha-net-poo-desafio).

## Objetivo

Modelar um sistema de celulares usando Programação Orientada a Objetos: uma abstração de smartphone que permite que diferentes marcas e modelos tenham comportamento próprio, reaproveitando código.

## Estrutura

```
Smartphone (abstrata)
├── Numero, Modelo, IMEI, Memoria
├── Ligar()
├── ReceberLigacao()
└── InstalarAplicativo(nome)  ← abstrato
    ├── Nokia   → instala pela Nokia Store
    └── Iphone  → instala pela App Store
```

## Conceitos aplicados

- **Abstração**: `Smartphone` é abstrata e não pode ser instanciada diretamente.
- **Herança**: `Nokia` e `Iphone` herdam propriedades e métodos de `Smartphone`.
- **Polimorfismo**: cada marca sobrescreve (`override`) `InstalarAplicativo` do seu jeito.
- **Encapsulamento**: `Modelo`, `IMEI` e `Memoria` são privados.

## Como executar

```bash
dotnet run
```
