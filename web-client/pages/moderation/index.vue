﻿<template>
  <v-card color="basil">
    <v-toolbar>
      <v-tabs v-model="tab" centered>
        <v-tab v-for="(t, i) in tabs" :key="`moderation-tab-${i}`">
          {{ t.title }}
        </v-tab>
      </v-tabs>
    </v-toolbar>

    <v-tabs-items v-model="tab">
      <v-tab-item v-for="(t, i) in tabs" :key="`moderation-tab-body-${i}`">
        <component :is="t.component"/>
      </v-tab-item>
    </v-tabs-items>
  </v-card>
</template>

<script>
import ModerationItemFeed from '@/components/moderation/moderation-item-feed'
import ModerationCategoryOverview from '@/components/moderation/moderation-category-overview'
import ModerationSubCategoryOverview from '@/components/moderation/moderation-subcategory-overview'


  export default {
    data: () => ({
      tab: 0,
      tabs: [
        {title: "Moderation", component: ModerationItemFeed},
        {title: "Categories", component: ModerationCategoryOverview},
        {title: "Sub-Categories", component: ModerationSubCategoryOverview}
      ]
    })
  }
</script>

<style scoped>

</style>
