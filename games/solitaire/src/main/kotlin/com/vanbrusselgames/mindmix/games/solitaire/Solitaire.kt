package com.vanbrusselgames.mindmix.games.solitaire

class Solitaire(val viewModel: SolitaireViewModel = SolitaireViewModel()) {
    companion object {
        const val GAME_ID: Int = 1
        const val GAME_NAME = "Solitaire"

        val NAME_RES_ID: Int = R.string.solitaire_name
        val IMAGE_RES_ID: Int = R.drawable.playingcards_detailed_clovers_a
    }
}