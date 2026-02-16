import org.jetbrains.kotlin.gradle.dsl.JvmTarget
import org.jetbrains.kotlin.gradle.tasks.KotlinJvmCompile

plugins {
    alias(libs.plugins.android.library)
    alias(libs.plugins.compose)
    alias(libs.plugins.ksp)
}

android {
    namespace = "com.vanbrusselgames.mindmix.core.ui"
    compileSdk = 36

    defaultConfig {
        minSdk = 13
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_21
        targetCompatibility = JavaVersion.VERSION_21
    }
    tasks.withType<KotlinJvmCompile>().configureEach {
        compilerOptions {
            jvmTarget.set(JvmTarget.JVM_21)
        }
    }
}

dependencies {
    implementation(project(":core:designsystem"))
    implementation(project(":core:common"))
    implementation(project(":core:utils"))
    implementation(project(":core:logging"))

    implementation(libs.androidx.compose.ui.tooling)
    implementation(libs.androidx.compose.ui.tooling.preview)

    implementation(platform(libs.androidx.compose.bom))
    implementation(libs.androidx.compose.material)
    implementation(libs.androidx.compose.material3)
    implementation(libs.androidx.compose.ui)
    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.navigation.compose)

    implementation(platform(libs.firebase.bom))
    implementation(libs.firebase.analytics)
}