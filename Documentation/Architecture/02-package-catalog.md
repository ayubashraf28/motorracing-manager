# Package Catalog

This file describes all local Unity packages under `UnityProject/Packages/`.

## Core

- Package: `com.alivastudio.motorracingmanager.core`
- Assembly: `MotorracingManager.Core`
- Purpose: primitives and shared low-level abstractions.
- Engine dependency: no.

## Domain

- Package: `com.alivastudio.motorracingmanager.domain`
- Assembly: `MotorracingManager.Domain`
- Depends on: `Core`
- Purpose: entities/value objects/domain interfaces.
- Engine dependency: no.

## Simulation

- Package: `com.alivastudio.motorracingmanager.sim`
- Assembly: `MotorracingManager.Sim`
- Depends on: `Core`, `Domain`
- Purpose: race/economy/progression simulation rules.
- Engine dependency: no.

## Application

- Package: `com.alivastudio.motorracingmanager.app`
- Assembly: `MotorracingManager.App`
- Depends on: `Core`, `Domain`, `Sim`
- Purpose: use-cases and orchestration.
- Engine dependency: no.

## Persistence

- Package: `com.alivastudio.motorracingmanager.persistence`
- Assembly: `MotorracingManager.Persistence`
- Depends on: `Core`, `Domain`, `Sim`, `App`
- Purpose: save/load and versioned state handling.
- Engine dependency: no (by design target).

## Unity Adapters

- Package: `com.alivastudio.motorracingmanager.unity`
- Assembly: `MotorracingManager.Unity`
- Depends on: `App`, `Persistence`
- Purpose: MonoBehaviour adapters and startup composition root.
- Engine dependency: yes.

## UI

- Package: `com.alivastudio.motorracingmanager.ui`
- Assembly: `MotorracingManager.UI`
- Depends on: `Domain`, `App`, `Unity`
- Purpose: view controllers/presenters/navigation.
- Engine dependency: yes.

## Versioning Convention

All local packages currently start at `0.1.0`; version bumps should follow semantic versioning semantics as the API surface grows.
