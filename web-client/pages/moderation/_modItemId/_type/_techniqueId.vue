﻿
<template>
  <div>
    <!-- Check for modItem "technique" -->
    <div v-if="modItem">
      <!-- Display modItem's description -->
      {{modItem.description}}
    </div>

    <v-row>
      <!-- * Comment section part -->
      <v-col cols="7">
        <!-- Injecting comment section component, dyn binding comments so we can display them, hooking sendComment event -->
        <!-- This is the place where moderator creates comment, and place where all replies are created aswell -->
        <!-- :comments is coming from comment-section prop -->
        <comment-section :comments="comments" @send="sendComment"/>
      </v-col>

      <!-- * Review section part -->
      <v-col cols="5">
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

          <!-- Review actions => buttons -->
          <v-card-actions class="justify-center">
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
    components: {CommentSection},
    // Local state
    data: () => ({
      modItem: null,
      comments: [],
      reviews: [],
      reviewComment: "",
      parentCommentId: 0
    }),

    // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
    created() {
      // We want extract data that we passed in from url params in /moderation/{}/{}/{}
      const {modItemId, type, techniqueId} = this.$route.params

      // Produce the endpoint based on url type parameter, e.g. techniques
      const endpoint = endpointResolver(type)

      // Assign endpoint to the modItemId in local state
      // Get dynamic API controller => response - data, based on URL parameters that we extracted
      // * Getting particular technique that we wanna moderate?
      this.$axios.$get(`/api/${endpoint}/${techniqueId}`)
        // Fire and forget
        // Then assign modItemId(technique) that came from GET req. to local state item
        .then(modItemId => this.modItem = modItemId)

      // * Getting comments for particular moderation item
      this.$axios.$get(`/api/moderation-items/${modItemId}/comments`)
        // Then assign comments that came from GET req. to local state comments arr
        .then(comments => this.comments = comments)

      // * Getting reviews for particular moderation item
      this.$axios.$get(`/api/moderation-items/${modItemId}/reviews`)
        // Then assign reviews that came from GET req. to local state reviews arr
        .then(reviews => this.reviews = reviews)
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
      }
    },

    methods: {
      sendComment(content) {
        // Extract moderation item id from URL param
        const {modItemId} = this.$route.params;

        // Returning promise | Creating comment for particular moderation item, based on his Id that's passed via URL
        // As data to save to our API, pass object with content prop, which is comment "" from local state, v-model, binding
        return this.$axios.$post(`/api/moderation-items/${modItemId}/comments`, {content: content})
          // Then push comment to local state arr of comments
          .then(comment => this.comments.push(comment));
      },

      // Depending on which button we press, status of that will be passed to createReview
      createReview(status) {
        // Extract moderation item id from URL param
        const {modItemId} = this.$route.params;

        return this.$axios.$post(`/api/moderation-items/${modItemId}/reviews`,
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
