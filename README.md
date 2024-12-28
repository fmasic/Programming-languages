# Sudoku (C#, JavaScript, Python)

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)

## Introduction
This project uses C#, JavaScript, and Python, and implements a sudoku game in each language. There is 9x9 grid, consisting of 3x3 subgrids that take digits 1 to 9 entered by the user,  an error checker and solver in each version. Every implementation utilizes similar logic and presentes similar output.

## Features
- **How To Play?**: New players can read brief instructions on how to play Sudoku, explaining the rules.
- **Puzzle Generation**: Every run provides a brand-new Sudoku problem that can be solved.
- **Real-Time Validation**: Player input is validated in real time to make sure it complies with Sudoku rules.
- **Automatic Solver**: Solves any Sudoku puzzle by using created algorithm.
- **Three chances**: User is allowed to make no more than three mistakes.
- **Clear board**: Deletes every input given by the user, allowing a fresh start.
- **Multi-Language**: Implemented in C#, Python, and JavaScript.

## Installation
Will be updated!

## Usage
1. **Starting the Game**:
     - Run the proper command to start the program.
     - The application will launch with randomly generated Sudoku problem.

2. **Playing the Game**:
    - A 9x9 grid format will be used to display the problem. The user has the option to enter their guesses for the numbers in the blank cells. 
    - The program will verify user input by looking for contradictions with the Sudoku rules, which prohibit repeating numbers in 3x3 subgrids, rows, or columns.

3. **Solving the Puzzle**:
     - The solver option can be used if the user wants to view the answer.
     - The finished puzzle will be located and shown by using created algorithm.

4. **Making Mistakes**
     - Each time a player makes a mistake, they have one less chance.
     - Upon making three mistakes, the game will end, resulting in the player losing.

5. **Exiting the Game**:
     - Dismiss the program window or follow the on-screen instructions to end the game.

