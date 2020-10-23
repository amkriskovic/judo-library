<template>
  <div>
    <v-row>
      <!-- * Comment section part -->
      <v-col cols="8">
        <v-row justify="center">
          <v-col cols="4" v-if="current">
            <technique-info-card :technique="current"/>
          </v-col>
          <v-col cols="4" class="d-flex justify-center" v-if="current">
            <v-icon size="46">mdi-arrow-right</v-icon>
          </v-col>
          <v-col cols="4" v-if="target">
            <technique-info-card :technique="target"/>
          </v-col>
        </v-row>

        <!-- Injecting comment section component, dyn binding comments so we can display them, hooking sendComment event -->
        <!-- This is the place where moderator creates comment, and place where all replies are created aswell -->
        <!-- :comments is coming from comment-section prop -->
        <comment-section :comments="comments" @send="sendComment"/>
      </v-col>

      <!-- * Review section part -->
      <v-col cols="4">
        <v-card>
          <!-- Displaying approved count -->
          <v-card-title>Reviews ({{approveCount}} / {{reviews.length}})</v-card-title>

          <!-- List of reviews -->
          <v-card-text>

            <!-- If there are any reviews in arr -->
            <div v-if="reviews.length > 0">
              <!-- Loop over them -->
              <div v-for="review in reviews" :key="`review-${review.id}`">
                <v-icon :color="reviewStatusColor(review.status)">{{reviewStatusIcon(review.status)}}</v-icon>
                Username
                <!-- If there is review comment, display it-->
                <span v-if="review.comment">{{review.comment}}</span>
              </div>
            </div>
            <!-- Else -->
            <div v-else>No Reviews</div>

            <v-divider class="my-3"></v-divider>

            <!-- This is for capturing review(Comment) content -->
            <v-text-field label="Review Comment" clearable v-model="reviewComment"></v-text-field>
          </v-card-text>

          <!-- Outdated section -->
          <div v-if="outdated">
            Outdated
          </div>

          <!-- Review actions => buttons -->
          <v-card-actions v-else class="justify-center">
            <!-- Loop over reviewActions arr and dynamically create buttons with corresponding titles and statuses -->
            <!-- :color =>> dynamic binding of colors for each button based on status from reviewAction arr -->
            <!-- onClick create review based on status, and passing content that's typed -->
            <v-btn v-for="reviewAction in reviewActions" :disabled="reviewAction.disabled"
                   :color="reviewStatusColor(reviewAction.status)" :key="`ra-${reviewAction.title}`"
                   @click="createReview(reviewAction.status)">

              <!-- Passing reviewStatusIcon method and passing to that method status from reviewActions arr -->
              <v-icon>{{reviewStatusIcon(reviewAction.status)}}</v-icon>
              {{reviewAction.title}}
            </v-btn>


            <!-- Passing reviewStatusIcon method and passing to that method status from reviewActions arr -->
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

  </div>
</template>

<script>
  // Separate from component, easier to re-use it
  // Resolves endpoint based on type it's passed, @ =>> root of web-client
  import CommentSection from "@/components/comments/comment-section";
  import TechniqueInfoCard from "@/components/technique-info-card";

  // Produce the endpoint based on url type parameter, e.g. techniques
  const endpointResolver = (type) => {
    // If type is technique, returns techniques string which we will use for resolving our API endpoint
    if (type === 'technique') return 'techniques'
  }

  // Reviews statuses, mimicking backend enum
  const REVIEW_STATUS = {
    APPROVED: 0,
    REJECTED: 1,
    WAITING: 2
  }

  // Depending on review status returns color corresponding to that status
  const reviewStatusColor = (reviewStatus) => {
    switch (reviewStatus) {
      case REVIEW_STATUS.APPROVED:
        return "green"
      case REVIEW_STATUS.REJECTED:
        return "red"
      case REVIEW_STATUS.WAITING:
        return "orange"
      default:
        return ""
    }
  }

  // Depending on review status returns icon corresponding to that status
  const reviewStatusIcon = (reviewStatus) => {
    switch (reviewStatus) {
      case REVIEW_STATUS.APPROVED:
        return "mdi-check"
      case REVIEW_STATUS.REJECTED:
        return "mdi-close"
      case REVIEW_STATUS.WAITING:
        return "mdi-clock"
      default:
        return ""
    }
  }

  export default {
    components: {TechniqueInfoCard, CommentSection},
    // Local state
    data: () => ({
      target: null,
      current: null,
      comments: [],
      reviews: [],
      reviewComment: "",
      parentCommentId: 0
    }),

    // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
    async created() {
      // We want extract data that we passed in from url params in /moderation/{modId.id}
      // Needs to be called modId coz of file => _modId.vue
      const {modId} = this.$route.params

      // Getting moderation item via GET req., passing modId from URL params
      const modItem = await this.$axios.$get(`/api/moderation-items/${modId}`)

      // Extract the comments from modItem and assign to page local state
      this.comments = modItem.comments

      // Extract the reviews from modItem and assign to page local state
      this.reviews = modItem.reviews

      // Produce the endpoint based on url type parameter, e.g. techniques => extract the type from modItem
      const endpoint = endpointResolver(modItem.type)

      // Assign endpoint to the item in local state
      // Get dynamic API controller => response - data, based on URL parameters that we extracted
      // * Provide current from modItem which is int => current version of item we editing =>> CURRENT
      this.$axios.$get(`/api/${endpoint}/${modItem.current}`)
        // Fire and forget
        // Then assign item(curr) that came from GET req. to local state item -> current
        .then(currItem => this.current = currItem)

      // * Make GET req to get the target(next) version =>> TARGET
      this.$axios.$get(`api/${endpoint}/${modItem.target}`)
        // Fire and forget
        // Then assign item(targetItem) that came from GET req. to local state item
        .then(targetItem => this.target = targetItem)
    },

    // Computed properties allow us to define a property that is used the same way as data,
    // but can also have some custom logic that is cached based on its dependencies.
    // You can consider computed properties another view into your data.
    computed: {
      // reviewActions represents computed prop, which returns arr of object with come custom props
      reviewActions() {
        return [
          {title: "Approved", status: REVIEW_STATUS.APPROVED, disabled: false}, // It's always enabled
          {title: "Rejected", status: REVIEW_STATUS.REJECTED, disabled: !this.reviewComment}, // Disabled if nothing is typed in input
          {title: "Wait", status: REVIEW_STATUS.WAITING, disabled: !this.reviewComment}, // Disabled if nothing is typed in input
        ]
      },

      // Returns number of approved reviews => length
      approveCount() {
        return this.reviews.filter(r => r.status === REVIEW_STATUS.APPROVED).length
      },

      // If target(next) - current is less than or equal to zero -> it's outdated
      outdated() {
        return this.current && this.target && this.target.version - this.current.version <= 0
      }
    },

    methods: {
      sendComment(content) {
        // Extract moderation item id from URL param
        const {modId} = this.$route.params;

        // Returning promise | Creating comment for particular moderation item, based on his Id that's passed via URL
        // As data to save to our API, pass object with content prop, which is comment "" from local state, v-model, binding
        return this.$axios.$post(`/api/moderation-items/${modId}/comments`, {content: content})
          // Then push comment to local state arr of comments
          .then(comment => this.comments.push(comment));
      },

      // Depending on which button we press, status of that will be passed to createReview
      createReview(status) {
        // Extract moderation item id from URL param
        const {modId} = this.$route.params;

        return this.$axios.$post(`/api/moderation-items/${modId}/reviews`,
          {
            // Data payload that we send to POST API, needs to mimick Review.cs
            comment: this.reviewComment,
            status: status
          })
          // Push created review to reviews arr
          .then(review => this.reviews.push(review))
      },

      // Bind review status "stylers" to the methods => so we can use them
      reviewStatusColor,
      reviewStatusIcon,
    },

  }
</script>

<style scoped>

</style>
