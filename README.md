# 🎰 Slot Machine Game

A 2D slot machine game developed in **Unity 6** as part of the Underpin Services Unity Game Development Assignment.

## 🎮 Game Overview

The game features a three-reel slot machine with four different symbols. Players select their bet, pull the lever, and try to land matching symbols to win coins.

The game uses weighted random symbol generation, smooth reel animations, sequential reel stopping, and a balance and payout system.

## ✨ Features

- **Three spinning reels:** Each reel displays three symbols at a time.
- **Winning combinations:** Matching all three symbols on the middle horizontal payline results in a win.
- **Weighted RNG:** Different symbols have different probabilities of appearing.
- **Smooth reel animations:** Reels scroll vertically, decelerate, and stop sequentially.
- **Bet selection:** Players can choose between 10, 50, and 100 coins.
- **Balance management:** Bets are deducted when a spin begins, and winnings are added to the balance.
- **Payout system:** Different symbols have different payout multipliers.
- **Interactive lever:** Clicking the lever initiates a spin.

## 🎲 Symbol Probabilities & Payouts

| Symbol | Probability | Payout Multiplier |
|---|---:|---:|
| Red Seven | 10% | 10x |
| Cherries | 20% | 5x |
| Bell | 30% | 3x |
| BAR | 40% | 2x |

The probability values are implemented using weights in the symbol result generator. Each reel generates its result independently.

A win occurs when all three symbols on the middle horizontal row match.

## 🕹️ How to Play

1. Start the game with an initial balance of 1,000 coins.
2. Select a bet amount: 10, 50, or 100 coins.
3. Click the lever to spin the reels.
4. Wait for the reels to stop.
5. If all three middle-row symbols match, receive the corresponding payout.
6. Continue playing while you have sufficient balance.

## 🛠️ Technical Implementation

**Engine:** Unity 6

**Architecture:**

- `SlotMachineController` – Coordinates spins, reel stopping, and result evaluation.
- `ReelController` – Handles symbol movement, recycling, deceleration, and stopping.
- `SlotResultGenerator` – Generates randomized symbol outcomes using weighted probabilities.
- `WinEvaluator` – Checks whether the middle-row symbols form a winning combination.
- `PayoutCalculator` – Calculates winnings based on the matched symbol and bet.
- `BalanceManager` – Manages player balance, bets, and winnings.
- `SymbolView` – Controls the displayed sprite for each symbol.
- `SlotMachineUI` – Updates balance, bet selection, and spin result messages.
- `BetButtonVisual` – Handles bet button selection and hover visuals.
- `LeverController` – Handles lever interaction and its pull animation.

The project uses object-oriented design principles to separate gameplay systems, reel behaviour, and UI responsibilities.

## 📁 Project Structure

```text
Assets/
├── Animations/
├── Art/
│   ├── Background/
│   ├── Machine/
│   ├── Symbols/
│   └── UI/
├── Prefabs/
│   └── Symbols/
├── Scenes/
├── Scripts/
│   ├── Core/
│   ├── Gameplay/
│   ├── Reels/
│   └── UI/
└── UI/
```

## 🌐 Running the WebGL Build

1. Open the GitHub repository.
2. Navigate to `Build/WebGL`.
3. Use the hosted WebGL build link, if available, or download the build.
4. To run a downloaded build locally, serve the folder using a local HTTP server rather than opening `index.html` directly.

## 💡 Development Approach

The project was developed by separating the slot machine into independent systems for random result generation, reel movement, winning condition evaluation, payouts, and balance management.

The reel animation uses symbol recycling to create continuous vertical movement. During stopping, the reel decelerates and aligns the symbols into their final positions.

Weighted random generation was introduced to make high-value symbols less frequent than lower-value symbols, creating a more varied slot machine experience.

The focus was on implementing the core gameplay loop, maintaining readable and modular code, and creating a functional user interface.

## 🚀 Built With

- Unity 6
- C#
- TextMesh Pro
- Git & GitHub