<template>
  <div>
    <v-row v-if="modItem">
      <!-- * Comment section part -->
      <v-col cols="8">
        <v-row justify="center">
          <v-col cols="5" v-if="current">
            <technique-info-card :technique="current"/>
          </v-col>
          <v-col cols="2" class="d-flex justify-center" v-if="current">
            <v-icon size="46">mdi-arrow-right</v-icon>
          </v-col>
          <v-col cols="5" v-if="target">
            <technique-info-card :technique="target"/>
          </v-col>
        </v-row>

        <v-divider class="my-2"/>

        <if-auth>
          <template v-slot:allowed>
            <div class="text-h4">Change Discussion</div>
            <comment-section :parent-id="modItem.id" :parent-type="moderationItemParentType"/>
          </template>
          <template v-slot:forbidden="{login}">
            <div class="d-flex justify-center">
              <v-btn outlined @click="login">sign in to join the discussion</v-btn>
            </div>
          </template>
        </if-auth>
      </v-col>

      <!-- * Review section part -->
      <v-col cols="4">
        <v-card>
          <v-card-text>
            <div v-if="reviews.length > 0">
              <div class="d-flex mb-2" v-for="review in reviews" :key="`review-${review.id}`">
                <div class="mr-3">
                  <v-badge bottom overlap :color="reviewStatusColor(review.status)"
                           :icon="reviewStatusIcon(review.status)">
                    <user-header :image-url="review.user.image"/>
                  </v-badge>
                </div>
                <div>
                  <div>{{ review.user.username }}</div>
                  <div class="body-1 white--text" v-if="review.comment">{{ review.comment }}</div>
                </div>
              </div>
            </div>

            <div class="d-flex justify-center" v-else>No Reviews</div>
            <v-divider class="my-3"></v-divider>

            <!-- IF mod -->
            <div v-if="moderator">
              <div v-if="outdated">
                Outdated
              </div>
              <v-select :label="'Review'" v-else v-model="review.status" :items="reviewActions">
                <template v-slot:item="{on, attrs, item}">
                  <v-list-item v-on="on" :attrs="attrs" :value="item.value">
                    <v-list-item-icon>
                      <v-icon :color="reviewStatusColor(item.value)">
                        {{ reviewStatusIcon(item.value) }}
                      </v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>{{ item.text }}</v-list-item-content>
                  </v-list-item>
                </template>
              </v-select>
              <v-dialog :value="review.status >= 0" persistent width="400">
                <v-card v-if="selectedReview">
                  <v-card-text class="pt-4">
                    <v-text-field label="Review Comment" v-model="review.comment"></v-text-field>
                  </v-card-text>
                  <v-card-actions>
                    <v-btn text @click="resetReviewForm">Cancel</v-btn>
                    <v-spacer/>
                    <v-btn :color="reviewStatusColor(review.status)"
                           :disabled="selectedReview.commentRequired && review.comment.length < 5"
                           @click="createReview">
                      {{ selectedReview.text }}
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-dialog>
            </div>
          </v-card-text>

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
import {COMMENT_PARENT_TYPE} from "@/components/comments/_shared";
import {modItemRenderer, REVIEW_STATUS} from "@/components/moderation";
import IfAuth from "@/components/auth/if-auth";
import UserHeader from "@/components/user-header";
import {mapGetters} from "vuex";

const initReview = () => ({
  status: -1,
  comment: ""
})

export default {
  components: {UserHeader, IfAuth, TechniqueInfoCard, CommentSection},

  mixins: [modItemRenderer],

  // Local state
  data: () => ({
    target: null,
    current: null,
    modItem: null,
    reviews: [],
    review: initReview()
  }),

  // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
  async fetch() {
    // We want extract data that we passed in from url params in /moderation/{modId.id}
    // Needs to be called modId coz of file => _modId.vue
    const {modId} = this.$route.params

    // Getting moderation item via GET req., passing modId from URL params
    this.modItem = await this.$axios.$get(`/api/moderation-items/${modId}`)

    const {type, current, target} = this.modItem

    // Produce the endpoint based on url type parameter, e.g. techniques => extract the type from modItem
    const endpoint = this.endpointResolver(type)

    const loadReviews = this.loadReviews()

    // Assign endpoint to the item in local state
    // Get dynamic API controller => response - data, based on URL parameters that we extracted
    // * Provide current from modItem which is int => current version of item we editing =>> CURRENT
    const loadCurrent = this.$axios.$get(`/api/${endpoint}/${current}`)
      // Fire and forget
      // Then assign item(curr) that came from GET req. to local state item -> current
      .then(currItem => this.current = currItem)

    // * Make GET req to get the target(next) version =>> TARGET
    const loadTarget = this.$axios.$get(`api/${endpoint}/${target}`)
      // Fire and forget
      // Then assign item(targetItem) that came from GET req. to local state item
      .then(targetItem => this.target = targetItem)

    await Promise.all([loadReviews, loadCurrent, loadTarget])
  },

  methods: {
    // Depending on which button we press, status of that will be passed to createReview
    createReview() {
      // Extract moderation item id from URL param
      const {modId} = this.$route.params;

      return this.$axios.$put(`/api/moderation-items/${modId}/reviews`,
        {
          comment: this.review.comment,
          status: this.review.status,
        })
        .then(this.loadReviews)
        .then(this.resetReviewForm)
    },

    loadReviews() {
      return this.$axios.$get(`/api/moderation-items/${this.modItem.id}/reviews`)
        .then((reviews) => this.reviews = reviews)
    },

    resetReviewForm() {
      this.review = initReview()
    }
  },

  // Computed properties allow us to define a property that is used the same way as data,
  // but can also have some custom logic that is cached based on its dependencies.
  // You can consider computed properties another view into your data.
  computed: {
    ...mapGetters('auth', ['moderator']),

    // reviewActions represents computed prop, which returns arr of object with come custom props
    reviewActions() {
      return [
        {text: "Approved", value: REVIEW_STATUS.APPROVED, commentRequired: false},
        {text: "Rejected", value: REVIEW_STATUS.REJECTED, commentRequired: true},
        {text: "Wait", value: REVIEW_STATUS.WAITING, commentRequired: true},
      ]
    },

    // Returns number of approved reviews => length
    approveCount() {
      return this.reviews.filter(r => r.status === REVIEW_STATUS.APPROVED).length
    },

    // If target(next) - current is less than or equal to zero -> it's outdated
    outdated() {
      return this.current && this.target && this.target.version - this.current.version <= 0
    },

    moderationItemParentType() {
      return COMMENT_PARENT_TYPE.MODERATION_ITEM
    },

    selectedReview() {
      const review = this.reviewActions.find(x => x.value === this.review.status)
      return review === undefined ? null : review
    }
  },

}
</script>

<style scoped>

</style>
