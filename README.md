# Saga Pattern in C# — Choreography & Orchestration Example

This repository contains a simple example of implementing the Saga pattern in C#, showcasing both Choreography and Orchestration approaches.

## 📖 Overview

The Saga pattern is used to manage long-lived transactions and maintain data consistency in distributed systems, especially in microservice architectures.
Instead of relying on a single, centralized transaction, Saga breaks the process into smaller steps and defines compensating actions to handle failures.

In this project, you'll find:

- ✅ Choreography-based Saga — services communicate through events without a central coordinator.
- ✅ Orchestration-based Saga — a central orchestrator coordinates the transaction flow.

## 🚀 How to Run

1. Clone the repository:
  ```bash
  git clone https://github.com/Pepegov/Sample.Net.SagaStateMachine.git
  cd Sample.Net.SagaStateMachine
  ```

2. Open the solution in Visual Studio or Rider.

3. Set the desired project (Choreography or Orchestration) as the startup project.
  For the orchestration, run the projects: 
  - Orchestration 
  - Orchestration.Delivery 
  - Orchestration.Inventory 
  - Orchestration.Order 

  In the Orchestration project window, press Enter to start the Saga 

  For the  choreography, run the projects: 
  - Choreography.Delivery
  - Choreography.Inventory
  - Choreography.Order 

  In the Choreography project Choreography.Order press Enter to start the Saga.

4. Run the application and follow the logs/output to see the Saga flow in action.

## 🎯 Goal

The purpose of this repository is to help beginners understand the Saga pattern in C# by providing both approaches side-by-side.
By running and comparing them, you’ll get a clear picture of how each works and when to use one over the other.

## 📬 Feedback & Contributions

If you have suggestions or improvements, feel free to open an issue or submit a pull request.