# Playnite Play Next Addon
 ![DownloadCountTotal](https://img.shields.io/github/downloads/sparrowbrain/playnite.playnext/total?label=total%20downloads&style=for-the-badge)
![LatestVersion](https://img.shields.io/github/v/release/SparrowBrain/Playnite.PlayNext?label=Latest%20version&style=for-the-badge)
![DownloadCountLatest](https://img.shields.io/github/downloads/SparrowBrain/Playnite.PlayNext/latest/total?style=for-the-badge)

## What Is It?
Play Next addon for Playnite (similar to Steam PlayNext).

Since we don't have big data like Steam, the recommendations are based on your activity (playtime, recent play order) as well as general properties (like critic score, community score or release year).

![Main NextPlay view screenshot](/ci/screenshots/01.jpg)
![Main NextPlay view screenshot](/ci/screenshots/02.jpg)
The recommendation score algorithm is highly customizable to your personal taste.

## Settings
![Main NextPlay view screenshot](/ci/screenshots/03.jpg)

Total Play Next score is calculated by weighting and adding up following attributes:

| Attribute | Value |
| --------- | ---------- |
| Genre | Attribute score |
| Feature | Attribute score |
| Developer | Attribute score |
| Publisher | Attribute score |
| Tag | Attribute score |
| Series | Attribute score weighted by order in series |
| Critic Score | Itself |
| Community Score | Itself |
| Release Year | How close to preferred year it is. It can either be the current year (ie newer games are preferred), or a specific one |
| Game Length | How close to preferred length it is. The length can be specified in hours. Requires HowLongToBeat addon ⚠️  |

Attribute score depends on your activity:

| Activity | Comment |
| --------- | ---------- |
| Total Playtime | All playtime |
| Recent Playtime | Playtime within recent days, requires GameActivity addon ⚠️ |
| Recent Order | Order of games played in recent days |

## Algorithm
Algorithm works in two steps:
1. It calculates scores for attributes (genre, feature, developer, publisher, tag);
2. It uses attribute score with additional game properties to calculate the final score.

### Attribute Score Calculation
#### Total Playtime
We iterate through all games that have any playtime. The attributes of the most played game get the maximum score while other games' attributes get proportionally smaller scores (depending on playtime).

#### Recent Playtime
Same as Total Playtime, except we use recent playtime withing configured recent days to get the attribute scores.

#### Recent Order
In a way it's a poor man's Recent Playtime. It just orders games played within recent days and assigns the highest score to most recent game's attributes, giving proportionally smaller scores to older game's attributes, ending with zero for the last one.

### Game Score Calculation

#### Attribute Scores
Adds up score to the game score using the different attribute scores calculated by playtime / recent order.

#### Series Scores
Same as attribute scores, except it's weighted by the order in the series. The first game in the series gets the maximum score, while the last one gets the smallest score. Order in series can be either release date or sorting name.

#### Critic Score
Just adds the game's critic score to the score. Zero if game has no critic score.

#### Community Score
Just adds the game's community score to the score. Zero if game has no community score.

#### Release Year
Takes release years for all games and then assigns the scores depending on the difference from the preferred year. If the game's release year matches, the game gets maximum score. The other release years earn proportionally less score, depending on the difference from preferred release year. The biggest difference will earn zero score.

For example if our prefered year is 2000 and our biggest difference game is from 2010, then we get these scores:
* Year 1990 - 0;
* Year 1995 - 50;
* Year 2000 - 100;
* Year 2005 - 50;
* Year 2010 - 0;

#### Game Length
Takes the game lengths for all games and assings the scores depending on the difference from the preferred length, capping out at half the preferred length.
For example if our preferred length is 20 hours, then we get these scores:
* Hours 0 - 0;
* Hours 10 - 0;
* Hours 15 - 50;
* Hours 20 - 100;
* Hours 25 - 50;
* Hours 30 - 0;

### Weighting
Weighting sets a maximum amount of score a specific part of algorigthm can contribute to the end result. That means in attribute score calculation weights set how much specific types influence the attribute scores. And in game score calculation we specify how much those attribute types influence the final score for games that will get recommended.

This lets us fine tune algorithm to get the games we prefer. For example we can aim for games with the same genre we played a lot and games with high critic scores.

## Installation
You can install it either from Playnite's addon browser, or from [the web addon browser](https://playnite.link/addons.html#SparrowBrain_PlayNext).

## Other Addons Integration
### StartPage
![Main NextPlay view screenshot](/ci/screenshots/04.jpg)

PlayNext has a custom view for [StartPage](https://github.com/felixkmh/StartPage-for-Playnite). It's designed to look similar to StartPage game shelves.

### GameActivity
In order to get attribute score calculation based on Recent Playtime to work you will need to have [GameActivity](https://github.com/Lacro59/playnite-gameactivity-plugin) addon installed.

### HowLongToBeat
In order to calculate the score depending on the game length you will need to have [HowLongToBeat](https://github.com/Lacro59/playnite-howlongtobeat-plugin) addon installed.

## Localization
You can help translate the extension to your language on the [Crowdin](https://crowdin.com/project/sparrowbrain-playnite-playnext) page.

