<template>
<div class="container">
    <div class="row justify-content-md-center mt-5">
        <div class="col-12">
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title">
                        Basic implementation for the 2Pay.js client
                    </h5>

                    <form type="post" id="payment-form">
                        <div class="form-group">
                            <label for="name" class="label control-label">Name</label>
                            <input type="text" v-model="billingDetails.name" class="field form-control" />
                        </div>

                        <div id="card-element">
                            <!-- A TCO IFRAME will be inserted here. -->
                        </div>

                        <button class="btn btn-primary" type="submit" @click.prevent="submit">
                            Generate token
                        </button>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
</template>

<script>
let jsPaymentClient = new window.TwoPayClient("AVLRNG");

// Set the desired language to be used inside the iframe.
jsPaymentClient.setup.setLanguage("ro");
let component = jsPaymentClient.components.create("card");
import {
    myhttp
} from "@/data/myhttp";
export default {
    mounted() {
        component.mount("#card-element");
    },
    data: function () {
        return {
            billingDetails: {
                name: "Veeran Kutty Puthumkara",
            },
        };
    },
    methods: {
        submit() {
            // Call the generate method using the component as the first parameter
            // and the billing details as the second one
            jsPaymentClient.tokens
                .generate(component, this.billingDetails)
                .then((response) => {
                    let token = {
                        id: "1",
                        token: response.token,
                    };
                    myhttp
                        .post("twocheckout/checkout", token)
                        .then(function (response) {
                            console.log(response);
                        })
                        .catch(function (error) {
                            console.log("My Error" + error);
                        });
                })
                .catch((error) => {
                    console.error(error);
                });
        },
    },
};
</script>

<style></style>
