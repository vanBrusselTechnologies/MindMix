import org.jetbrains.kotlin.gradle.dsl.JvmTarget
import org.jetbrains.kotlin.gradle.tasks.KotlinJvmCompile

plugins {
    alias(libs.plugins.android.library)
    alias(libs.plugins.ksp)
    alias(libs.plugins.kotlin.serialization)
    alias(libs.plugins.hilt)
}

android {
    namespace = "com.vanbrusselgames.mindmix.core.data"
    compileSdk = 36
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
    implementation(project(":core:logging"))
    implementation(project(":core:model"))

    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.datastore.preferences)
    implementation(libs.hilt)
    ksp(libs.hilt.compiler)
    implementation(libs.kotlinx.serialization.json)

    implementation(platform(libs.firebase.bom))
    implementation(libs.firebase.storage)
    implementation(libs.firebase.authentication)
}