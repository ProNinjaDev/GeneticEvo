# Evolutionary Genetic Algorithm for Job Sequencing

This repository contains an implementation of an **Evolutionary Genetic Algorithm (EGA)** designed to solve a **job sequencing problem**. The algorithm uses selection, crossover, and mutation operators to evolve a population of candidate solutions over several generations, aiming to minimize the makespan of job execution across machines.

## Features
- **Customizable genetic operators**: Selection, mutation, and crossover mechanisms can be tailored to your needs.
- **Support for large job-machine matrices**: Handles input data in `.DAT` files.
- **Replacement strategy**: Dynamically replaces less fit individuals while maintaining population diversity.
- **Statistics tracking**: Monitors average fitness, best fitness, and standard deviation for every generation.

## Getting Started

### Prerequisites
- **.NET SDK**: Ensure that you have the .NET 6 SDK or higher installed on your machine. You can download it from the official [Microsoft .NET website](https://dotnet.microsoft.com/).

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/ProNinjaDev/GeneticEvo.git
   cd GeneticEvo
2.	Open the project in your preferred IDE (e.g., Visual Studio, JetBrains Rider, or Visual Studio Code).
3.	Build and run the project:
4.	dotnet build
5.	dotnet run
