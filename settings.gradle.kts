pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
    }
}
rootProject.name = "MindMix"
include(":app")

include(":core:advertisement")
//include(":core:analytics")
include(":core:authentication")
include(":core:common")
include(":core:data")
//include(":core:data-test")
//include(":core:database")
//include(":core:datastore")
//include(":core:datastore-proto")
//include(":core:datastore-test")
include(":core:designsystem")
//include(":core:domain")
include(":core:logging")
//include(":core:model")
include(":core:navigation")
//include(":core:network")
//include(":core:notifications")
//include(":core:screenshot-testing")
//include(":core:testing")
include(":core:ui")
include(":core:utils")

include(":feature:gamefinished")
include(":feature:gamehelp")
include(":feature:gamemenu")
include(":feature:menu")
include(":feature:settings")

include(":games:game2048")
include(":games:minesweeper")
include(":games:solitaire")
include(":games:sudoku")
