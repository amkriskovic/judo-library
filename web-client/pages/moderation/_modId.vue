﻿<template>
  <div>
    <v-row v-if="modItem">
      <!-- * Comment section part -->
      <v-col cols="8">
        <v-row justify="center">
          <v-col cols="5" v-if="current">
            <component v-if="itemComponent" :[itemComponent.payload]="current" :is="itemComponent.is"/>
          </v-col>
          <v-col cols="2" class="d-flex justify-center" v-if="current && target">
            <v-icon size="46">mdi-arrow-right</v-icon>
          </v-col>

          <v-col cols="5" v-if="target">
            <component v-if="itemComponent" :[itemComponent.payload]="target" :is="itemComponent.is"
                       @edit="itemComponent.edit(target)"/>
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
import SimpleInfoCard from "@/components/moderation/simple-info-card";
import CategoryForm from "@/components/content-creation/category-form"
import SubCategoryForm from "@/components/content-creation/subcategory-form"
import {COMMENT_PARENT_TYPE} from "@/components/comments/_shared";
import {MODERATION_TYPES, modItemRenderer, REVIEW_STATUS, VERSION_STATE} from "@/components/moderation";
import IfAuth from "@/components/auth/if-auth";
import UserHeader from "@/components/user-header";
import {mapActions, mapGetters, mapMutations} from "vuex";
import {EVENTS} from "@/data/events";
import {onContentUpdate} from "@/components/_shared";

const initReview = () => ({
  status: -1,
  comment: ""
})

export default {
  components: {UserHeader, IfAuth, TechniqueInfoCard, CommentSection},

  mixins: [modItemRenderer, onContentUpdate],

  // Local state
  data: () => ({
    target: null,
    current: null,
    modItem: null,
    reviews: [],
    review: initReview()
  }),

  async fetch() {
    const {modId} = this.$route.params

    this.modItem = await this.$axios.$get(`/api/moderation-items/${modId}`)

    await this.loadRelationships()
  },

  methods: {
    onContentUpdate() {
      return this.loadRelationships()
    },

    loadRelationships() {
      const {type, current, target} = this.modItem
      const endpoint = this.endpointResolver(type)
      const loadReviews = this.loadReviews()
      const loadCurrent = this.$axios.$get(`/api/${endpoint}/${current}`)
        .then((item) => this.current = item)
      const loadTarget = this.$axios.$get(`/api/${endpoint}/${target}`)
        .then((item) => this.target = item)

      return Promise.all([loadReviews, loadCurrent, loadTarget])
    },

    createReview() {
      const {modId} = this.$route.params;

      return this.$axios.$put(`/api/moderation-items/${modId}/reviews`,
        {
          comment: this.review.comment,
          status: this.review.status,
        })
        .then(this.loadReviews)
        .then(this.resetReviewForm)
        .then(() => this.$nuxt.$emit(EVENTS.CONTENT_UPDATED))
        .then(this.loadContent)
    },

    loadReviews() {
      return this.$axios.$get(`/api/moderation-items/${this.modItem.id}/reviews`)
        .then((reviews) => this.reviews = reviews)
    },

    resetReviewForm() {
      this.review = initReview()
    },

    ...mapActions('library', ['loadContent']),
    ...mapMutations('content-creation', ['activate'])
  },

  computed: {
    ...mapGetters('auth', ['moderator']),

    reviewActions() {
      return [
        {text: "Approve", value: REVIEW_STATUS.APPROVED, commentRequired: false},
        {text: "Reject", value: REVIEW_STATUS.REJECTED, commentRequired: true},
        {text: "Wait", value: REVIEW_STATUS.WAITING, commentRequired: true},
      ]
    },

    approveCount() {
      return this.reviews.filter(r => r.status === REVIEW_STATUS.APPROVED).length
    },

    outdated() {
      return this.current && this.target && this.current.state === VERSION_STATE.OUTDATED
    },

    moderationItemParentType() {
      return COMMENT_PARENT_TYPE.MODERATION_ITEM
    },

    selectedReview() {
      const review = this.reviewActions.find(x => x.value === this.review.status)
      return review === undefined ? null : review
    },

    itemComponent() {
      if (!this.modItem) return null;

      if (this.modItem.type === MODERATION_TYPES.TECHNIQUE)
        return {is: TechniqueInfoCard, payload: 'technique'};

      if (this.modItem.type === MODERATION_TYPES.CATEGORY)
        return {
          is: SimpleInfoCard,
          payload: 'payload',
          edit: (category) => this.activate({component: CategoryForm, editPayload: category})
        };
      if (this.modItem.type === MODERATION_TYPES.SUBCATEGORY)
        return {
          is: SimpleInfoCard,
          payload: 'payload',
          edit: (subcategory) => this.activate({component: SubCategoryForm, editPayload: subcategory})
        };

      return null;
    }
  },

}
</script>

<style scoped>

</style>
