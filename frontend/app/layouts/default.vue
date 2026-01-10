<template>
    <div class="layout">
        <Header />
        <main class="main-content">
            <slot />
        </main>
        <Footer />
    </div>
</template>

<style scoped lang="scss">
.layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.main-content {
  flex: 1;
}
</style>