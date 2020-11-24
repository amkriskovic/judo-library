<template>
  <div>

    <div>
      <h3 class="text-h3 text-center">Techniques</h3>
      <front-page-technique-feed />
    </div>

    <v-divider class="my-5"></v-divider>

    <div>
      <h3 class="text-h3 text-center">Categories</h3>
      <front-page-category-feed />
    </div>

    <v-divider class="my-5"></v-divider>

    <div>
      <h3 class="text-h3 text-center">Sub Categories</h3>
      <front-page-subcategory-feed />
    </div>

  </div>
</template>

<script>
  import {mapState} from "vuex"
  import FrontPageTechniqueFeed from "@/components/front-page/front-page-technique-feed";
  import FrontPageCategoryFeed from "@/components/front-page/front-page-category-feed";
  import FrontPageSubcategoryFeed from "@/components/front-page/front-page-subcategory-feed";

  export default {
    components: {FrontPageSubcategoryFeed, FrontPageCategoryFeed, FrontPageTechniqueFeed},
  }
</script>
