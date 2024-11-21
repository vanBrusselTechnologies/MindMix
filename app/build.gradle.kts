import java.io.FileInputStream
import java.util.Properties

plugins {
    alias(libs.plugins.android.application)
    alias(libs.plugins.ksp)
    alias(libs.plugins.compose)
    alias(libs.plugins.kotlin.android)
    id(libs.plugins.firebase.crashlytics.get().pluginId)
    alias(libs.plugins.firebase.perf)
    alias(libs.plugins.gms)
    alias(libs.plugins.kotlin.serialization)
}

val keystorePropertiesFile: File = rootProject.file("keystore.properties")
val keystoreProperties = Properties()
keystoreProperties.load(FileInputStream(keystorePropertiesFile))

android {
    signingConfigs {
        create("release") {
            keyAlias = keystoreProperties["keyAlias"] as String
            keyPassword = keystoreProperties["keyPassword"] as String
            storeFile = file(keystoreProperties["storeFile"] as String)
            storePassword = keystoreProperties["storePassword"] as String
        }
    }
    namespace = "com.vanbrusselgames.mindmix"
    compileSdk = 35

    defaultConfig {
        applicationId = "com.vanBrusselGames.MindMix"
        minSdk = 23
        targetSdk = 35
        versionCode = 102
        versionName = "1.2.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
        vectorDrawables {
            useSupportLibrary = true
        }
    }

    buildTypes {
        getByName("release") {
            isMinifyEnabled = true
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"), "proguard-rules.pro"
            )
            signingConfig = signingConfigs.getByName("release")
        }
        getByName("debug") {
            applicationIdSuffix = ".debug"
            signingConfig = signingConfigs.named("debug").get()
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    kotlinOptions {
        jvmTarget = "17"
    }
    buildFeatures {
        compose = true
    }
    composeOptions {
        kotlinCompilerExtensionVersion = "1.5.15"
    }
    packaging {
        resources {
            excludes += "/META-INF/{AL2.0,LGPL2.1}"
        }
    }
}

dependencies {
    implementation(project(":core:advertisement"))
    implementation(project(":core:authentication"))
    implementation(project(":core:common"))
    implementation(project(":core:data"))
    implementation(project(":core:designsystem"))
    implementation(project(":core:logging"))
    implementation(project(":core:navigation"))
    implementation(project(":core:utils"))
    implementation(project(":feature:gamefinished"))
    implementation(project(":feature:gamehelp"))
    implementation(project(":feature:gamemenu"))
    implementation(project(":feature:menu"))
    implementation(project(":feature:settings"))
    implementation(project(":games:game2048"))
    implementation(project(":games:minesweeper"))
    implementation(project(":games:solitaire"))
    implementation(project(":games:sudoku"))

    implementation(libs.androidx.activity.compose)
    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.lifecycle.runtime.ktx)

    implementation(platform(libs.androidx.compose.bom))
    implementation(libs.androidx.compose.ui)
    implementation(libs.androidx.compose.ui.tooling)
    implementation(libs.androidx.compose.material3)
    implementation(libs.androidx.constraintlayout)
    implementation(libs.androidx.datastore.preferences)
    implementation(libs.androidx.navigation.compose)
    implementation(libs.kotlinx.serialization.json)

    implementation(platform(libs.firebase.bom))
    implementation(libs.firebase.appcheck.playintegrity)
    implementation(libs.firebase.config)
    implementation(libs.firebase.crashlytics)
    implementation(libs.firebase.performance)

    implementation(libs.play.services.ads)
    implementation(libs.play.services.auth)
    implementation(libs.play.services.games.v2)
    implementation(libs.review)
    implementation(libs.review.ktx)
    implementation(libs.app.update)
    implementation(libs.app.update.ktx)

    testImplementation(libs.junit)
    androidTestImplementation(libs.ext.junit)
    androidTestImplementation(libs.androidx.test.espresso.core)
    androidTestImplementation(platform(libs.androidx.compose.bom))
    androidTestImplementation(libs.androidx.ui.test.junit4)
    debugImplementation(libs.androidx.compose.ui.tooling)
    debugImplementation(libs.androidx.ui.test.manifest)
}