
let API = (user_id, token, end_cursor, id_post) => {
    return new Promise((resolve, reject) => {
        fetch("https://www.facebook.com/api/graphql/",
            {
                "headers": {
                    "accept": "/",
                    "accept-language": "en-US,en;q=0.9",
                    "content-type": "application/x-www-form-urlencoded",
                    "sec-ch-prefers-color-scheme": "light",
                    "sec-ch-ua": "\".Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"",
                    "sec-ch-ua-mobile": "?0",
                    "sec-ch-ua-platform": "\"Windows\"", "sec-fetch-dest": "empty", "sec-fetch-mode": "cors", "sec-fetch-site": "same-origin",
                },
                "referrer": document.URL,
                "referrerPolicy": "origin-when-cross-origin",
                "body": 'av=' + user_id + '&__user=' + user_id + '&__a=1&fb_dtsg=' + token + '&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=CometUFICommentsProviderForDisplayCommentsQuery&variables={"UFI2CommentsProvider_commentsKey":"CometGroupPermalinkRootFeedQuery","__false": false,"__true": true,"after":"' + end_cursor + '","before": null,"displayCommentsContextEnableComment": null,"displayCommentsContextIsAdPreview": null,"displayCommentsContextIsAggregatedShare": null,"displayCommentsContextIsStorySet": null,"displayCommentsFeedbackContext": null,"feedLocation":"GROUP_PERMALINK","feedbackSource": 2,"first": 50,"focusCommentID": null,"includeHighlightedComments": false,"includeNestedComments": true,"initialViewOption":"TOPLEVEL","isInitialFetch": false,"isPaginating": true,"last": null,"scale": 1.5,"topLevelViewOption": null,"useDefaultActor": false,"viewOption": null,"id":"' + btoa("feedback:" + id_post) + '" }&server_timestamps=true&doc_id=5315135448570862',
                "method": "POST",
                "mode": "cors",
                "credentials": "include"
            })
            .then((data) => { return data })
            .then((data2) => {
                data2.text().then((data) => {
                    try {
                        let obj = JSON.parse(data);
                        resolve(JSON.stringify(obj));
                    } catch (e) {
                        let obj = JSON.parse(data.substring(0, data.indexOf('{"label":"VideoPlaye')));
                        resolve(JSON.stringify(obj));
                    }
                });
            });
    });
};
return API(require("CurrentUserInitialData").USER_ID, require("DTSGInitialData").token, "%<NEXT>%", "%<POST>%");