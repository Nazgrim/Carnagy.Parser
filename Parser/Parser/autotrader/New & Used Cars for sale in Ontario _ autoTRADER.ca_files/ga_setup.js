var CP_Google_Analytics = function () {
    var _self = this;

    this.logger = function (value) {
        if (readCookie("debug") == "true") {
            console.log("cp: " + value);
        }
    }
    this.createCookie = function (name, value, days) {
        var domain = "";
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + Math.ceil(days * 24 * 60 * 60 * 1000));
            var expires = " expires=" + date.toGMTString() + ";";
        }

        if (typeof (cookieDomain) != 'undefined')
            domain = " domain=" + cookieDomain + "; ";
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    };

    this.readCookie = function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    this.detectSearch = function () {
        var search = false;
        if (document.referrer.indexOf(location.hostname) >= 0 && tm_waData.hasOwnProperty('searchCat') && !this.readCookie("searchFlag")) {
            search = true
        }
        if (tm_waData.pv.indexOf("/ad-details/") >= 0) {
            this.createCookie("searchFlag", "true", 1);
            this.logger("ad-details: flag added");
        }
        return search
    }

    this.getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    this.trackSearch = function () {
        if (this.detectSearch()) {
            var label = "";
            label += tm_waData.searchCat
            var str = tm_waData.pv.split("|");
            make = (str[2] != null) ? str[2].replace(/"/g, "") : "";
            model = (str[3] != null) ? str[3].split('&')[0].replace(/"/g, "") : "";
            label = label + "|" + make + "|" + model;

            if (getParameterByName("loc") != "") {
                label += "|" + getParameterByName("loc");
            }

            _gaq.push(['_trackEvent', 'Search', 'Unique Searches-new', label, 0, true]);
            this.logger("Unique search -> " + label);
        }

        if (tm_waData.hasOwnProperty('searchCat')) {
            this.createCookie("searchFlag", "true", 1);
            this.logger("flag added");
            $(function () {
                $(".searchCriteriaOptions select,.searchCriteriaOptions input,span.at_closeBtn").on("change", function () {
                    _self.createCookie("searchFlag", "", -1);
                    _self.logger("flag removed");
                });
                $(".searchCriteriaOptions input,span.at_closeBtn").on("click", function () {
                    _self.createCookie("searchFlag", "", -1);
                    _self.logger("flag removed");
                });
            });
        }

        if (tm_waData.hasOwnProperty('searchCat') == false && tm_waData.pv.indexOf("/ad-details/") < 0) {
            _self.createCookie("searchFlag", "", -1);
            _self.logger("flag removed: not search or details page");
        }
        $(function () {
            $("#recent_search_list a,#breadcrumbs a").on("click", function () {
                _cpga.createCookie("searchFlag", "", -1);
                _cpga.logger("flag removed");
            });
        });
    }

    if (this.getParameterByName("debug") == "true") {
        this.createCookie("debug", "true", 5);
        this.logger("enable logger");

    }
    if (this.getParameterByName("debug") == "false") {
        this.logger("disable logger");
        this.createCookie("debug", "true", -1);
    }
}
var _cpga = new CP_Google_Analytics();

function wa_trackAllowedParams(allowed, params, allowVals) {
    var i, ret = '',
        paramsRE = new RegExp('(' + allowed + ')=(.*)', 'i'),
        matches = null,
        searchParams = params.split('|'),
        len = searchParams.length;
    for (i = 0; i < len; i++) {
        matches = paramsRE.exec(searchParams[i]);
        if (matches !== null) {
            ret += matches[1];
            if (allowVals) ret += '=' + matches[2];
            ret += ',';
        }
    }
    return ret.substr(0, ret.length - 1);
}

function wa_trackSearchParameters(params, cat) {
    var allowedParams = wa_trackAllowedParams('year|price|mileage|cond|type|recommended|photo', params, true);
    if (allowedParams != '') {
        _gaq.push(['_trackEvent', 'Search', 'Additional Parameters:' + allowedParams, cat + '|advanced', 0, true]);
    }
}

function wa_trackSearchRefinements(params, type, cat) {
    var allowedParams = wa_trackAllowedParams('year|price|mileage|hour|length|vehicle condition|cond|class|subclass|category|province|city|make|model|colour|bodyType|transmission|seller|distance|listingsWithPrice|keyword|type|distance|postalCode|listingsWithFreeCarProof|fuel', params, false);
    if (allowedParams != '') {
        _gaq.push(['_trackEvent', 'Search', 'Refined Parameters:' + allowedParams, cat + '|' + type, 0, true]);
    }
}

function wa_trackUniqueSearch(type) {
    _gaq.push(['_trackEvent', 'Search', 'Unique Searches', type, 0, true]);
}

function wa_trackAdModification(action) {
    _gaq.push(['_trackEvent', 'Ad Modification', action]);
}

function wa_trackToolbarClick(item) {
    _gaq.push(['_trackEvent', 'ToolBar', item]);
}

function wa_trackNavClick(item) {
    _gaq.push(['_trackEvent', 'Global Nav', item]);
}

function wa_trackSearchClick(item) {
    _gaq.push(['_trackEvent', 'Search', item]);
}

function wa_trackUnlink(social_network) {
    _gaq.push(['_trackEvent', 'Social Unlink', social_network]);
}

function wa_trackSocialSignInClick(social_network) {
    _gaq.push(['_trackEvent', 'Social Sign In', social_network]);
}

function wa_trackManageAccountAction(action, period) {
    _gaq.push(['_trackEvent', 'Manage Account', action, period || '']);
}

function wa_trackFailedSearch(cat, type, make, model, loc, prox) {
    _gaq.push(['_trackEvent', 'Search', 'Failed Search', cat + '|' + type, 0, true]);
    wa_failedSearch = true;
}

function wa_trackGeoExpansion(cat, type, make, model, loc, prox, expandedLoc, expandedProx) {
    if (!wa_failedSearch) {
        _gaq.push(['_trackEvent', 'Search', 'Geo-Expansion', cat + '|' + type, 0, true]);
    }
}

function wa_trackContactPoster(cat, status, adtype, attributes, region, isupsell, showcpo, upsell, sendPV, price, emailMessage, sourcePage, pricePhoto, strikeThroughPricing, vehicleSpecialist, previousNextBar, adId, dealerId, csContent, hasDealerReview, isPhotoGalleryModal) {
    _gaq.push(['_setCustomVar', 6, adId, "Ad ID", 3]);
    if (dealerId != "")
        _gaq.push(['_setCustomVar', 7, dealerId, "Dealer ID", 3]);

    var mobialsValue = hasDealerReview === 'True' ? 'Yes' : 'No';
    _gaq.push(['_setCustomVar', 8, 'MobialsLead', mobialsValue, 3]);

    var csLabel = '';
    var actionType = '';
    var actionSuffix = '';
    var pageViewSuffix = '';
    if (csContent) {
        var csContentSplitted = csContent.split(';');
        pageViewSuffix = '-' + csContentSplitted[0];
        pageViewSuffix += '-' + (WaData.isDetailPage === true ? (isPhotoGalleryModal === true ? 'modal' : 'vdp') : 'serp');
        actionType = csContentSplitted[1];
        actionSuffix += '|CS-Type=' + actionType;
        actionSuffix += '|CS-Loc=' + (WaData.isDetailPage === true ? 'VDP' : 'SERP');
        csLabel = csContentSplitted[2];
    }
    var i, pv = '/ad-tools/contact-poster-complete' + pageViewSuffix + '/',
        fields = ['adCat', 'adStatus', 'adType', 'adAttributes', 'adProv', 'adHasUpsell', 'adShowCPO', 'adUpsellType', 'adStrikethroughPrice', 'adVehicleSpecialist', 'adCompareTool', 'adPreviousNextBar'];

    if (sendPV) {
        if (typeof WaData == 'object') {
            for (i = 0; i < fields.length; i++) {
                if (fields[i] in WaData) pv = pv + WaData[fields[i]] + '|';
                else pv = pv + '|';
            }
        }
        _gaq.push(['_trackPageview', pv]);
    };
    var priceText = (price == null ? "" : price);
    var emailMsg = (emailMessage == null ? "" : emailMessage);
    var incentiveTypes = (sourcePage == 'DetailsPage') ? GetIncentiveTypesOnAdDetails() : [];
    var adDetailsIncentiveFlags = (sourcePage == 'DetailsPage') ? GetAdDetailsIncentive(incentiveTypes) : '||';
    var pricePhotoText = (pricePhoto == null ? "" : pricePhoto);
    var tdiFlags = (sourcePage == 'DetailsPage') ? GetAdDetailsTdiInfos() : '|||';
    var strikeThroughPricingText = (strikeThroughPricing != null ? strikeThroughPricing : '');
    var vehicleSpecialistText = (vehicleSpecialist != null ? vehicleSpecialist : '');
    var previousNextBarText = (previousNextBar != null ? previousNextBar : '');
    var compareToolText = 'COMP=' + (WaData.isDetailPage === true ? '0' : '1');
    _gaq.push(['_trackEvent', 'Ads', 'Contact Poster' + actionSuffix, cat + '|' + status + '|' + adtype + '|' + attributes + '|' + region + '|' + isupsell + '|' + showcpo + '|' + upsell + '|' + priceText + '|' + emailMsg + adDetailsIncentiveFlags + '|' + pricePhotoText + tdiFlags + '|' + strikeThroughPricingText + '|' + vehicleSpecialistText + '|' + previousNextBarText + '|' + compareToolText + '|' + csLabel, incentiveTypes.length, true]);
    _gaq.push(['_trackEvent', 'Contact Poster Type', 'AdType=' + WaData.adType + '|CS-Type=' + actionType, csLabel, 0, true]);
}

function wa_trackContactPosterByRequestType(cat, status, adtype, attributes, region, isupsell, showcpo, upsell, pageView, evtAction, sendPV, price, emailMessage, sourcePage, pricePhoto, strikeThroughPricing, vehicleSpecialist, previousNextBar, adId, dealerId, csContent, hasDealerReview) {
    _gaq.push(['_setCustomVar', 6, adId, "Ad ID", 3]);
    if (dealerId != "")
        _gaq.push(['_setCustomVar', 7, dealerId, "Dealer ID", 3]);
    var mobialsValue = hasDealerReview === 'True' ? 'Yes' : 'No';
    _gaq.push(['_setCustomVar', 8, 'MobialsLead', mobialsValue, 3]);
    
    var csLabel = '';
    var actionType = '';
    var actionSuffix = '';
    var pageViewSuffix = '';
    if (csContent) {
        var csContentSplitted = csContent.split(';');
        pageViewSuffix = '-' + csContentSplitted[0];
        pageViewSuffix += '-' + (WaData.isDetailPage === true ? 'vdp' : 'serp');
        actionType = csContentSplitted[1];
        actionSuffix += '|CS-Type=' + actionType;
        actionSuffix += '|CS-Loc=' + (WaData.isDetailPage === true ? 'VDP' : 'SERP');
        csLabel = csContentSplitted[2];
    }
    var i, pv = '/ad-tools/' + pageView + pageViewSuffix + '/',
        fields = ['adCat', 'adStatus', 'adType', 'adAttributes', 'adProv', 'adHasUpsell', 'adShowCPO', 'adUpsellType', 'adStrikethroughPrice', 'adVehicleSpecialist', 'adCompareTool', 'adPreviousNextBar'];
    if (sendPV) {
        if (typeof WaData == 'object') {
            for (i = 0; i < fields.length; i++) {
                if (fields[i] in WaData) pv = pv + WaData[fields[i]] + '|';
                else pv = pv + '|';
            }
        }
        _gaq.push(['_trackPageview', pv]);
    };
    var priceText = (price == null ? "" : price);
    var emailMsg = (emailMessage == null ? "" : emailMessage);
    var incentiveTypes = (sourcePage == 'DetailsPage') ? GetIncentiveTypesOnAdDetails() : [];
    var adDetailsIncentiveFlags = (sourcePage == 'DetailsPage') ? GetAdDetailsIncentive(incentiveTypes) : '||';
    var pricePhotoText = (pricePhoto == null ? "" : pricePhoto);
    var tdiFlags = (sourcePage == 'DetailsPage') ? GetAdDetailsTdiInfos() : '|||';
    var strikeThroughPricingText = (strikeThroughPricing != null ? strikeThroughPricing : '');
    var vehicleSpecialistText = (vehicleSpecialist != null ? vehicleSpecialist : '');
    var previousNextBarText = (previousNextBar != null ? previousNextBar : '');
    var compareToolText = 'COMP=' + (WaData.isDetailPage === true ? '0' : '1');
    _gaq.push(['_trackEvent', 'Ads', evtAction + actionSuffix, cat + '|' + status + '|' + adtype + '|' + attributes + '|' + region + '|' + isupsell + '|' + showcpo + '|' + upsell + '|' + priceText + '|' + emailMsg + adDetailsIncentiveFlags + '|' + pricePhotoText + tdiFlags + '|' + strikeThroughPricingText + '|' + vehicleSpecialistText + '|' + previousNextBarText + '|' + compareToolText + '|' + csLabel, incentiveTypes.length, true]);
    _gaq.push(['_trackEvent', 'Contact Poster Type', 'AdType=' + WaData.adType + '|CS-Type=' + actionType, csLabel, 0, true]);
}

function wa_trackMVTTest(testInfo) {
    _gaq.push(function () {
        var i, name = 'MVT',
            pageTracker = _gat._getTrackerByName(),
            currentVal = pageTracker._getVisitorCustomVar(4),
            newVal = '!' + testInfo,
            MVTs = [],
            numMVTs = 0,
            testID, testCombo, testInfoArr = testInfo.split('*');
        testID = testInfoArr[0];
        testCombo = testInfoArr[1];
        if (typeof currentVal == 'string') {
            MVTs = currentVal.split('!');
            numMVTs = MVTs.length;
            for (i = numMVTs - 1; i >= 0; i--) {
                if (MVTs[i].search(testID + '\*') > -1) continue;
                if (('!' + MVTs[i] + newVal).length < (64 - name.length)) newVal = '!' + MVTs[i] + newVal;
                else break;
            }
        }
        newVal = newVal.slice(1);
        _gaq.push(['_setCustomVar', 4, 'MVT', newVal, 1]);
    });
}

function wa_trackIncomingLinks(camp) {
    _gaq.push(['_setCustomVar', 1, 'Incoming_Links', camp, 2]);
}

function wa_trackInternalCampaigns(name, placement) {
    _gaq.push(['_setCustomVar', 5, 'AT-Services-Ads', name + '|' + placement, 1]);
    _gaq.push(['_trackEvent', 'AT Services Ads', 'Click on ad', name + '|' + placement, 0, true]);
}

function wa_trackFaceBook(event) {
    _gaq.push(['_trackEvent', 'facebook', event, ((WaData.pv) ? WaData.pv : '')]);
    _gaq.push(['_trackSocial', 'facebook', event, ((WaData.pv) ? WaData.pv : '')]);
}

function wa_trackPlus1(b) {
    var a = (b && b.state == 'on') ? '+1' : '-1';
    _gaq.push(['_trackEvent', 'googleplusone', a, ((WaData.pv) ? WaData.pv : '')]);
}

function wa_trackDistanceChange(distance) {
    _gaq.push(['_trackEvent', 'Changed-Distance', distance]);
}

function wa_trackDisplayPageSize(value) {
    _gaq.push(['_trackEvent', 'Changed-Number of Search Results', value, 'Search Results Changed']);
}

function wa_trackCoBrandSearch() {
    _gaq.push(['_trackEvent', 'Co-Brand Search', 'Search', ((WaData.pv) ? WaData.pv : ''), 0, true]);
}

function wa_trackSortSelectionChange(sortmethod) {
    _gaq.push(['_trackEvent', 'Changed-Sort', sortmethod]);
}

function wa_trackShowPhone(sellerType) {
    var type = (typeof sellerType == 'string') ? 'Display - ' + sellerType : 'Display';
    _gaq.push(['_trackEvent', 'Phone Number Display', type, ((WaData.pv) ? WaData.pv : '')]);
}

function wa_trackShowPhoneNew(adType, sourcePage, pricePhoto) {
    var adDetail = '';
    var incentiveTypes = (sourcePage == 'DetailsPage') ? GetIncentiveTypesOnAdDetails() : [];
    var type = (typeof adType == 'string') ? 'Display - ' + adType : 'Display';
    adDetail += GetAdDetailsIncentive(incentiveTypes) + '|';
    if (pricePhoto != null)
        adDetail += pricePhoto;
    adDetail += (sourcePage == 'DetailsPage') ? GetAdDetailsTdiInfos() : '|||';
    _gaq.push(['_trackEvent', 'Phone Number Display', type, ((WaData.pv) ? WaData.pv : '') + adDetail, incentiveTypes.length]);
}

function wa_trackShowEmailToFriendForm() {
    _gaq.push(['_trackPageview', '/ad-tools/email-to-friend']);
}

function wa_trackEmailToFriendComplete() {
    _gaq.push(['_trackPageview', '/ad-tools/email-to-friend-complete']);
}

function wa_trackDSSLink(dealerId) {
    _gaq.push(['_trackEvent', 'DSS Links', 'DSS Link Clicked', dealerId]);
}

function wa_trackNPSScore(rating) {
    _gaq.push(['_setCustomVar', 2, 'NPS-Score', rating, 1]);
}

function wa_trackFeedBackForm() {
    _gaq.push(['_trackEvent', 'Deprecated WA Function', 'wa_trackFeedBackForm']);
}

function wa_trackMultiPhotoView(fullUrl) {
    _gaq.push(['_setCustomVar', 20, "Full Url", fullUrl, 3]);

    _gaq.push(['_trackPageview', '/ad-details/multi-photo-view']);
}

function wa_trackAdDetailView(link, adDetailPrefix, pricePhoto) {
    var adDetail = adDetailPrefix;
    var incentiveTypes = GetIncentiveTypesOnResultItem(link);
    adDetail += GetAdDetailsIncentive(incentiveTypes) + '|';
    if (pricePhoto != null)
        adDetail += pricePhoto;
    adDetail += '|||'; //TDI Pipes |<Options>|<Packages>|<VehicleHighlights> (we don't track that in Details Page) [User Story: 34955]
    _gaq.push(['_trackEvent', 'Ad details', 'Clicked', adDetail, incentiveTypes.length]);
}

function wa_trackAdDetailPageView(adDetailPrefix) {
    _gaq.push(['_trackPageview', adDetailPrefix]);
}

function wa_trackPhotoStripClick(source) {
    _gaq.push(['_trackEvent', 'PhotoStrip', 'PhotoStripClick', source]);
}

function wa_trackReportAbuseClick(abuseType) {
    _gaq.push(['_trackEvent', 'ReportAbuse', 'ReportAbuseClick', abuseType]);
}

function wa_trackCaptchaError() {
    _gaq.push(['_trackEvent', 'Captcha', 'CaptchaError', '']);
}

function wa_trackDealerLink(dealerLinkParams) {
    _gaq.push(['_trackEvent', 'DealerLinks', 'Dealer Link Clicked', dealerLinkParams]);
}

function wa_trackFreeCarProofOnly() {
    _gaq.push(['_trackEvent', 'CarProof', 'Search Free CarProof Only', '']);
}

function wa_trackWithHasCustomPhoto() {
    _gaq.push(['_trackEvent', 'NewCar', 'Search With Has Custom Photo', '']);
}
function wa_trackWithIsRecommended() {
    _gaq.push(['_trackEvent', 'NewCar', 'Search With Is Recommended', '']);
}

function wa_trackCarProofGetReport(reportType) {
    _gaq.push(['_trackEvent', 'CarProof', 'Buy Report', reportType]);
}

function wa_trackCarProofPurchased(reportType, adId, dealerId) {
    _gaq.push(['_setCustomVar', 6, adId, "Ad ID", 3]);
    if (dealerId != "")
        _gaq.push(['_setCustomVar', 7, dealerId, "Dealer ID", 3]);
    _gaq.push(['_trackEvent', 'CarProof', 'Purchased Report', reportType]);
}

function wa_trackCarProofContactSeller(type, adId, dealerId, hasDealerReview) {
    _gaq.push(['_setCustomVar', 6, adId, "Ad ID", 3]);
    if (dealerId != "")
        _gaq.push(['_setCustomVar', 7, dealerId, "Dealer ID", 3]);
    var mobialsValue = hasDealerReview === 'True' ? 'Yes' : 'No';
    _gaq.push(['_setCustomVar', 8, 'MobialsLead', mobialsValue, 3]);
    _gaq.push(['_trackEvent', 'CarProof', 'Contact Seller', type]);
}

function wa_trackViewCarProofReport(type, adId, dealerId, hasDealerReview) {
    _gaq.push(['_setCustomVar', 6, adId, "Ad ID", 3]);
    if (dealerId != "")
        _gaq.push(['_setCustomVar', 7, dealerId, "Dealer ID", 3]);
    var mobialsValue = hasDealerReview === 'True' ? 'Yes' : 'No';
    _gaq.push(['_setCustomVar', 8, 'MobialsLead', mobialsValue, 3]);
    _gaq.push(['_trackEvent', 'CarProof', 'View CarProof Report', type]);
}

function wa_trackCarFlix(source) {
    _gaq.push(['_trackEvent', 'CarFlix', 'CarFlix Views', source]);
}

function wa_trackIpadHomePage() {
    _gaq.push(['_trackEvent', 'Ipad', 'Ipad Views', '/mobile/ipadhomepage']);
}

function wa_trackReviewHelpful(isHelpful, make, model) {
    var helpful = isHelpful ? 'Helpful' : 'Not-helpful';
    _gaq.push(['_trackEvent', 'Reviews', helpful, make + '|' + model, 0, true]);
}

function wa_trackReviewReadmore(make, model) {
    _gaq.push(['_trackEvent', 'Reviews', 'Readmore', make + '|' + model, 0, true]);
}

function wa_trackReviewSocial(source, make, model) {
    _gaq.push(['_trackEvent', 'Reviews', source, make + '|' + model, 0, true]);
}

function wa_trackReviewChangeSort(sortmethod) {
    _gaq.push(['_trackEvent', 'Reviews', 'Changed-sort', sortmethod, 0, true]);
}

function wa_trackReviewChangePagesize(pagesize) {
    _gaq.push(['_trackEvent', 'Reviews', 'Changed-pagesize', pagesize, 0, true]);
}

function wa_trackPromos(actn, lbl) {
    _gaq.push(['_trackEvent', 'Promos', actn, lbl]);
}

function wa_trackBodyType(actn, lbl) {
    _gaq.push(['_trackEvent', 'Search', actn, lbl]);
}

function wa_trackSellLandingStartNow(actn, lbl) {
    _gaq.push(['_trackEvent', 'Promos', actn, lbl]);
}

function wa_trackErrorPage(actn, lbl) {
    _gaq.push(['_trackEvent', 'Error', actn, lbl]);
}

function wa_trackSEOLandingPageClick(actn, lbl) {
    _gaq.push(['_trackEvent', 'SEO Landing Pages', actn, lbl]);
}

function wa_trackMarketingLPEvent(actn, lbl) {
    _gaq.push(['_trackEvent', 'Landing Page Events', actn, lbl]);
}

function wa_trackMarketplaceVideoEvent(actn, lbl) {
    _gaq.push(['_trackEvent', 'Marketplace Video', actn, lbl, 0, true]);
}

function wa_trackEvent(cat, actn, lbl) {
    _gaq.push(['_trackEvent', cat, actn, lbl]);
}

function wa_trackSignoutClick()
{
    _gaq.push(['_setCustomVar', 3, "Anonymous", "N/A", 2]);
}

function wa_trackNewsFeatures(actn, lbl) {
    _gaq.push(['_trackEvent', 'News and Features', actn, lbl]);
}

function wa_trackAdChoicesClickEvent(languageCode) {
    var action = 'Clicked on AdChoices' + ' ' + languageCode;
    _gaq.push(['_trackEvent', 'Exit Links', action, 'AdChoices', , true]);
}
function wa_trackDealerReviewClickEvent(page) {
    var label = 'Dealer Reviews' + ' ' + page;
    _gaq.push(['_trackEvent', 'Mobials', 'Clicked on Reviews', label, , true]);
}
function wa_trackEventNoInteraction(cat, actn, lbl, value) {
    _gaq.push(['_trackEvent', cat, actn, lbl, value, true]);
}

var wa_version = '1.15',
    wa_domain = '',
    wa_id = '',
    wa_host = document.location.hostname,
    wa_query = document.location.search,
    wa_xdomainlist = 'autotrader.ca|autohebdo.net|canadatrader.com|buysell.com',
    wa_xsubdomainlist = '/sell.|/vendre.',
    wa_failedSearch = false;
if (wa_host.indexOf('vkistudios.net') > -1) {
    wa_domain = 'vkistudios.net';
    wa_id = 'UA-5002093-1';
} else {
    if (wa_host.indexOf('autohebdo.net') > -1) {
        wa_domain = 'autohebdo.net';
    }
    else if (wa_host.indexOf('buysell.com') > -1) {
        wa_domain = 'buysell.com';
    } else {
        wa_domain = 'autotrader.ca';
    }
    wa_id = 'UA-10401800-1';
}
var _gaq = _gaq || [];
_gaq.push(['_setAccount', wa_id]);
_gaq.push(['_setDomainName', '.' + wa_domain]);
_gaq.push(['_setAllowLinker', true]);
_gaq.push(['_setAllowHash', false]);
_gaq.push(['_setAllowAnchor', true]);
_gaq.push(['_setSiteSpeedSampleRate', 50]);
_gaq.push(['_addIgnoredRef', 'app.fluidsurveys.com']);
_gaq.push(['_addIgnoredRef', 'autotrader.ca']);
_gaq.push(['_addIgnoredRef', 'buysell.com']);
_gaq.push(['_addIgnoredRef', 'autos.ca']);
_gaq.push(['_addIgnoredRef', 'autohebdo.net']);
if ((document.cookie.search(/__utmb/) == -1 || document.cookie.search(/__utmc/) == -1)) {
    if (wa_host.search(/wwwa|sella|vendrea/) > -1) _gaq.push(['_setVar', 'control']);
    else if (wa_host.search(/wwwb|sellb|vendreb/) > -1) _gaq.push(['_setVar', 'test']);
}
// var tm_waData = tmParam.tm_waData || {};
var tm_waData = WaData || {};
if (typeof tm_waData.NPSData == 'string') wa_trackNPSScore(tm_waData.NPSData);
if (typeof tm_waData.MVTData == 'string') wa_trackMVTTest(tm_waData.MVTData);
if (typeof tm_waData.feedbackData == 'string') wa_trackFeedBackForm(tm_waData.feedbackData);
if (wa_query.search(/from-listing-alert=true/) > -1) {
    wa_trackIncomingLinks('Listing_Alert');
}

if (typeof tm_waData.signinAuth === 'string' && typeof tm_waData.signinStatus === 'string') {
    _gaq.push(['_setCustomVar', 3, tm_waData.signinStatus, tm_waData.signinAuth, 2]);
}

if (typeof tm_waData.uniqueUserId === 'string') {
    _gaq.push(['_setCustomVar', 8, tm_waData.uniqueUserId, "User ID", 1]);
}
if (typeof tm_waData.pv == 'string') {
    if (tm_waData.isDetailPage) {
        var adId = tm_waData.adId;
        var dealerId = tm_waData.dealerId;
        var mobialsValue = tm_waData.hasDealerReview === 'True' ? 'Yes' : 'No';
        _gaq.push(['_setCustomVar', 6, adId, "Ad ID", 3]);
        if (dealerId != "")
            _gaq.push(['_setCustomVar', 7, dealerId, "Dealer ID", 3]);
        _gaq.push(['_setCustomVar', 8, 'MobialsDV', mobialsValue, 3]);
    }
    tm_waData.pv = tm_waData.pv.replace(/(search_cat=.*)( & )([^&]*)/, '$1_$3');
    _gaq.push(['_trackPageview', tm_waData.pv]);

} else _gaq.push(['_trackPageview']);
//_gaq.push(['_trackPageLoadTime']); deprecated
if (typeof tm_waData.uniqueSearch == 'boolean' && tm_waData.uniqueSearch) wa_trackUniqueSearch(tm_waData.searchType);
if (typeof tm_waData.failedSearch == 'boolean' && tm_waData.failedSearch) wa_trackFailedSearch(tm_waData.searchCat, tm_waData.searchType, tm_waData.make, tm_waData.model, tm_waData.loc, tm_waData.prox);
if (typeof tm_waData.searchParams == 'string') wa_trackSearchParameters(tm_waData.searchParams, tm_waData.searchCat);
if (typeof tm_waData.refinementFields == 'string') wa_trackSearchRefinements(tm_waData.refinementFields, tm_waData.searchType, tm_waData.searchCat);
if (typeof tm_waData.geoExpanded == 'boolean' && tm_waData.geoExpanded) wa_trackGeoExpansion(tm_waData.searchCat, tm_waData.searchType, tm_waData.make, tm_waData.model, tm_waData.loc, tm_waData.prox, tm_waData.expandedLoc, tm_waData.expandedProx);
if (typeof tm_waData.cobrandSearch == 'boolean' && tm_waData.cobrandSearch) wa_trackCoBrandSearch();
if (typeof tm_waData.contactPoster == 'boolean' && tm_waData.contactPoster) wa_trackContactPoster(tm_waData.adCat, tm_waData.adType, tm_waData.adAttributes, tm_waData.adProv, tm_waData.adCity, tm_waData.adUpsells);
if (typeof tm_waData.signinComplete === 'string') {
    _gaq.push(['_setCustomVar', 3, 'SignedIn', tm_waData.signinComplete, 2]);
    _gaq.push(['_trackEvent', 'Social Sign In', 'complete', tm_waData.signinComplete]);
}
if (typeof tm_waData.itMatrixData == 'string') wa_trackEventNoInteraction("IT Matrix", "Search Engine", tm_waData.itMatrixData);
if (typeof tm_waData.isLastPage == 'boolean' && tm_waData.isLastPage) wa_trackEventNoInteraction("IT Matrix", "Search Engine", "Last Page");
if (typeof tm_waData.resultPageType == 'string' && typeof tm_waData.resultOrganicCount == 'number') wa_trackEventNoInteraction("List Views", tm_waData.resultPageType, "Organic", tm_waData.resultOrganicCount);
if (typeof tm_waData.resultPageType == 'string' && typeof tm_waData.resultsPplsCount == 'number') wa_trackEventNoInteraction("List Views", tm_waData.resultPageType, "Listing Upgrades - PPL", tm_waData.resultsPplsCount);
if (typeof tm_waData.resultPageType == 'string' && typeof tm_waData.resultsPlsCount == 'number') wa_trackEventNoInteraction("List Views", tm_waData.resultPageType, "Listing Upgrades - PL", tm_waData.resultsPlsCount);
if (typeof tm_waData.resultPageType == 'string' && typeof tm_waData.resultsFlsCount == 'number') wa_trackEventNoInteraction("List Views", tm_waData.resultPageType, "Listing Upgrades - FL", tm_waData.resultsFlsCount);
if (typeof tm_waData.resultPageType == 'string' && typeof tm_waData.resultsSearchAlternativeCount == 'number') wa_trackEventNoInteraction("List Views", tm_waData.resultPageType, "Search Alternatives", tm_waData.resultsSearchAlternativeCount);
if (typeof tm_waData.linkingComplete == 'string') _gaq.push(['_trackEvent', 'Social Sign In', 'link', tm_waData.linkingComplete]);
if (typeof tm_waData.accountAutomaticallyCreated == 'boolean' && tm_waData.accountAutomaticallyCreated) _gaq.push(['_trackEvent', 'Social Sign In', 'pre-generate account']);
if (typeof tm_waData.blockedAccount == 'boolean' && tm_waData.blockedAccount) _gaq.push(['_trackEvent', 'Account Blocked', 'Account Blocked']);
// Start DIL Code
"function" !== typeof window.DIL && (window.DIL = function (a, c) {
    var d = [], b, g; a !== Object(a) && (a = {}); var e, h, k, q, p, n, l, D, m, J, K, E; e = a.partner; h = a.containerNSID; k = a.iframeAttachmentDelay; q = !!a.disableDestinationPublishingIframe; p = a.iframeAkamaiHTTPS; n = a.mappings; l = a.uuidCookie; D = !0 === a.enableErrorReporting; m = a.visitorService; J = a.declaredId; K = !0 === a.removeFinishedScriptsAndCallbacks; E = !0 === a.delayAllUntilWindowLoad; var L, M, N, F, C, O, P; L = !0 === a.disableScriptAttachment; M = !0 === a.disableCORSFiring; N = !0 === a.disableDefaultRequest;
    F = a.afterResultForDefaultRequest; C = a.dpIframeSrc; O = !0 === a.testCORS; P = !0 === a.useJSONPOnly; D && DIL.errorModule.activate(); var Q = !0 === window._dil_unit_tests; (b = c) && d.push(b + ""); if (!e || "string" !== typeof e) return b = "DIL partner is invalid or not specified in initConfig", DIL.errorModule.handleError({ name: "error", message: b, filename: "dil.js" }), Error(b); b = "DIL containerNSID is invalid or not specified in initConfig, setting to default of 0"; if (h || "number" === typeof h) h = parseInt(h, 10), !isNaN(h) && 0 <= h && (b = "");
    b && (h = 0, d.push(b), b = ""); g = DIL.getDil(e, h); if (g instanceof DIL && g.api.getPartner() === e && g.api.getContainerNSID() === h) return g; if (this instanceof DIL) DIL.registerDil(this, e, h); else return new DIL(a, "DIL was not instantiated with the 'new' operator, returning a valid instance with partner = " + e + " and containerNSID = " + h); var y = { IS_HTTPS: "https:" === document.location.protocol, POST_MESSAGE_ENABLED: !!window.postMessage, COOKIE_MAX_EXPIRATION_DATE: "Tue, 19 Jan 2038 03:14:07 UTC" }, G = { stuffed: {} }, u = {}, r = {
        firingQueue: [],
        fired: [], firing: !1, sent: [], errored: [], reservedKeys: { sids: !0, pdata: !0, logdata: !0, callback: !0, postCallbackFn: !0, useImageRequest: !0 }, callbackPrefix: "demdexRequestCallback", firstRequestHasFired: !1, useJSONP: !0, abortRequests: !1, num_of_jsonp_responses: 0, num_of_jsonp_errors: 0, num_of_cors_responses: 0, num_of_cors_errors: 0, corsErrorSources: [], num_of_img_responses: 0, num_of_img_errors: 0, toRemove: [], removed: [], readyToRemove: !1, platformParams: { d_nsid: h + "", d_rtbd: "json", d_jsonv: DIL.jsonVersion + "", d_dst: "1" }, nonModStatsParams: {
            d_rtbd: !0,
            d_dst: !0, d_cts: !0, d_rs: !0
        }, modStatsParams: null, adms: {
            TIME_TO_CATCH_ALL_REQUESTS_RELEASE: 2E3, calledBack: !1, mid: null, noVisitorAPI: !1, instance: null, releaseType: "no VisitorAPI", admsProcessingStarted: !1, process: function (f) {
                try {
                    if (!this.admsProcessingStarted) {
                        var s = this, a, x, b, d, c; if ("function" === typeof f && "function" === typeof f.getInstance) {
                            if (m === Object(m) && (a = m.namespace) && "string" === typeof a) x = f.getInstance(a); else { this.releaseType = "no namespace"; this.releaseRequests(); return } if (x === Object(x) && "function" ===
                            typeof x.isAllowed && "function" === typeof x.getMarketingCloudVisitorID) {
                                if (!x.isAllowed()) { this.releaseType = "VisitorAPI not allowed"; this.releaseRequests(); return } this.instance = x; this.admsProcessingStarted = !0; b = function (f) { "VisitorAPI" !== s.releaseType && (s.mid = f, s.releaseType = "VisitorAPI", s.releaseRequests()) }; Q && (d = m.server) && "string" === typeof d && (x.server = d); c = x.getMarketingCloudVisitorID(b); if ("string" === typeof c && c.length) { b(c); return } setTimeout(function () {
                                    "VisitorAPI" !== s.releaseType && (s.releaseType =
                                    "timeout", s.releaseRequests())
                                }, this.TIME_TO_CATCH_ALL_REQUESTS_RELEASE); return
                            } this.releaseType = "invalid instance"
                        } else this.noVisitorAPI = !0; this.releaseRequests()
                    }
                } catch (e) { this.releaseRequests() }
            }, releaseRequests: function () { this.calledBack = !0; r.registerRequest() }, getMarketingCloudVisitorID: function () { return this.instance ? this.instance.getMarketingCloudVisitorID() : null }, getMIDQueryString: function () {
                var f = v.isPopulatedString, s = this.getMarketingCloudVisitorID(); f(this.mid) && this.mid === s || (this.mid =
                s); return f(this.mid) ? "d_mid=" + this.mid + "&" : ""
            }
        }, declaredId: {
            declaredId: { init: null, request: null }, declaredIdCombos: {}, setDeclaredId: function (f, s) {
                var a = v.isPopulatedString, x = encodeURIComponent; if (f === Object(f) && a(s)) { var b = f.dpid, d = f.dpuuid, c = null; if (a(b) && a(d)) { c = x(b) + "$" + x(d); if (!0 === this.declaredIdCombos[c]) return "setDeclaredId: combo exists for type '" + s + "'"; this.declaredIdCombos[c] = !0; this.declaredId[s] = { dpid: b, dpuuid: d }; return "setDeclaredId: succeeded for type '" + s + "'" } } return "setDeclaredId: failed for type '" +
                s + "'"
            }, getDeclaredIdQueryString: function () { var f = this.declaredId.request, s = this.declaredId.init, a = ""; null !== f ? a = "&d_dpid=" + f.dpid + "&d_dpuuid=" + f.dpuuid : null !== s && (a = "&d_dpid=" + s.dpid + "&d_dpuuid=" + s.dpuuid); return a }
        }, registerRequest: function (f) {
            var s = this.firingQueue; f === Object(f) && s.push(f); this.firing || !s.length || E && !DIL.windowLoaded || (this.adms.calledBack ? (f = s.shift(), f.src = f.src.replace(/demdex.net\/event\?d_nsid=/, "demdex.net/event?" + this.adms.getMIDQueryString() + "d_nsid="), v.isPopulatedString(f.corsPostData) &&
            (f.corsPostData = f.corsPostData.replace(/^d_nsid=/, this.adms.getMIDQueryString() + "d_nsid=")), A.fireRequest(f), this.firstRequestHasFired || "script" !== f.tag && "cors" !== f.tag || (this.firstRequestHasFired = !0)) : this.processVisitorAPI())
        }, processVisitorAPI: function () { this.adms.process(window.Visitor) }, requestRemoval: function (f) {
            if (!K) return "removeFinishedScriptsAndCallbacks is not boolean true"; var s = this.toRemove, a, b; f === Object(f) && (a = f.script, b = f.callbackName, (a === Object(a) && "SCRIPT" === a.nodeName || "no script created" ===
            a) && "string" === typeof b && b.length && s.push(f)); if (this.readyToRemove && s.length) { b = s.shift(); a = b.script; b = b.callbackName; "no script created" !== a ? (f = a.src, a.parentNode.removeChild(a)) : f = a; window[b] = null; try { delete window[b] } catch (d) { } this.removed.push({ scriptSrc: f, callbackName: b }); DIL.variables.scriptsRemoved.push(f); DIL.variables.callbacksRemoved.push(b); return this.requestRemoval() } return "requestRemoval() processed"
        }
    }; g = function () {
        var f = "http://fast.", a = "?d_nsid=" + h + "#" + encodeURIComponent(document.location.href);
        if ("string" === typeof C && C.length) return C + a; y.IS_HTTPS && (f = !0 === p ? "https://fast." : "https://"); return f + e + ".demdex.net/dest4.html" + a
    }; var z = {
        THROTTLE_START: 3E4, throttleTimerSet: !1, id: "destination_publishing_iframe_" + e + "_" + h, url: g(), iframe: null, iframeHasLoaded: !1, sendingMessages: !1, messages: [], messagesPosted: [], messageSendingInterval: y.POST_MESSAGE_ENABLED ? 15 : 100, jsonProcessed: [], attachIframe: function () {
            var f = this, a = document.createElement("iframe"); a.id = this.id; a.style.cssText = "display: none; width: 0; height: 0;";
            a.src = this.url; t.addListener(a, "load", function () { f.iframeHasLoaded = !0; f.requestToProcess() }); document.body.appendChild(a); this.iframe = a
        }, requestToProcess: function (f, a) { var b = this; f && !v.isEmptyObject(f) && this.process(f, a); this.iframeHasLoaded && this.messages.length && !this.sendingMessages && (this.throttleTimerSet || (this.throttleTimerSet = !0, setTimeout(function () { b.messageSendingInterval = y.POST_MESSAGE_ENABLED ? 15 : 150 }, this.THROTTLE_START)), this.sendingMessages = !0, this.sendMessages()) }, process: function (f,
        a) {
            var b = encodeURIComponent, d, c, e, g, h, n; a === Object(a) && (n = t.encodeAndBuildRequest(["", a.dpid || "", a.dpuuid || ""], ",")); if ((d = f.dests) && d instanceof Array && (c = d.length)) for (e = 0; e < c; e++) g = d[e], g = [b("dests"), b(g.id || ""), b(g.y || ""), b(g.c || "")], this.addMessage(g.join("|")); if ((d = f.ibs) && d instanceof Array && (c = d.length)) for (e = 0; e < c; e++) g = d[e], g = [b("ibs"), b(g.id || ""), b(g.tag || ""), t.encodeAndBuildRequest(g.url || [], ","), b(g.ttl || ""), "", n], this.addMessage(g.join("|")); if ((d = f.dpcalls) && d instanceof Array && (c =
            d.length)) for (e = 0; e < c; e++) g = d[e], h = g.callback || {}, h = [h.obj || "", h.fn || "", h.key || "", h.tag || "", h.url || ""], g = [b("dpm"), b(g.id || ""), b(g.tag || ""), t.encodeAndBuildRequest(g.url || [], ","), b(g.ttl || ""), t.encodeAndBuildRequest(h, ","), n], this.addMessage(g.join("|")); this.jsonProcessed.push(f)
        }, addMessage: function (f) { var a = encodeURIComponent, a = D ? a("---destpub-debug---") : a("---destpub---"); this.messages.push(a + f) }, sendMessages: function () {
            var f = this, a; this.messages.length ? (a = this.messages.shift(), DIL.xd.postMessage(a,
            this.url, this.iframe.contentWindow), this.messagesPosted.push(a), setTimeout(function () { f.sendMessages() }, this.messageSendingInterval)) : this.sendingMessages = !1
        }
    }, I = {
        traits: function (f) { v.isValidPdata(f) && (u.sids instanceof Array || (u.sids = []), t.extendArray(u.sids, f)); return this }, pixels: function (f) { v.isValidPdata(f) && (u.pdata instanceof Array || (u.pdata = []), t.extendArray(u.pdata, f)); return this }, logs: function (f) {
            v.isValidLogdata(f) && (u.logdata !== Object(u.logdata) && (u.logdata = {}), t.extendObject(u.logdata,
            f)); return this
        }, customQueryParams: function (f) { v.isEmptyObject(f) || t.extendObject(u, f, r.reservedKeys); return this }, signals: function (f, a) { var b, d = f; if (!v.isEmptyObject(d)) { if (a && "string" === typeof a) for (b in d = {}, f) f.hasOwnProperty(b) && (d[a + b] = f[b]); t.extendObject(u, d, r.reservedKeys) } return this }, declaredId: function (f) { r.declaredId.setDeclaredId(f, "request"); return this }, result: function (f) { "function" === typeof f && (u.callback = f); return this }, afterResult: function (f) {
            "function" === typeof f && (u.postCallbackFn =
            f); return this
        }, useImageRequest: function () { u.useImageRequest = !0; return this }, clearData: function () { u = {}; return this }, submit: function () { A.submitRequest(u); u = {}; return this }, getPartner: function () { return e }, getContainerNSID: function () { return h }, getEventLog: function () { return d }, getState: function () {
            var f = {}, a = {}; t.extendObject(f, r, { callbackPrefix: !0, useJSONP: !0, registerRequest: !0 }); t.extendObject(a, z, { attachIframe: !0, requestToProcess: !0, process: !0, sendMessages: !0 }); return {
                pendingRequest: u, otherRequestInfo: f,
                destinationPublishingInfo: a
            }
        }, idSync: function (f) {
            if (f !== Object(f) || "string" !== typeof f.dpid || !f.dpid.length) return "Error: config or config.dpid is empty"; if ("string" !== typeof f.url || !f.url.length) return "Error: config.url is empty"; var a = f.url, b = f.minutesToLive, d = encodeURIComponent, c, a = a.replace(/^https:/, "").replace(/^http:/, ""); if ("undefined" === typeof b) b = 20160; else if (b = parseInt(b, 10), isNaN(b) || 0 >= b) return "Error: config.minutesToLive needs to be a positive number"; c = t.encodeAndBuildRequest(["",
            f.dpid, f.dpuuid || ""], ","); f = ["ibs", d(f.dpid), "img", d(a), b, "", c]; z.addMessage(f.join("|")); r.firstRequestHasFired && z.requestToProcess(); return "Successfully queued"
        }, aamIdSync: function (f) { if (f !== Object(f) || "string" !== typeof f.dpuuid || !f.dpuuid.length) return "Error: config or config.dpuuid is empty"; f.url = "//dpm.demdex.net/ibs:dpid=" + f.dpid + "&dpuuid=" + f.dpuuid; return this.idSync(f) }, passData: function (f) { if (v.isEmptyObject(f)) return "Error: json is empty or not an object"; A.defaultCallback(f); return "json submitted for processing" },
        getPlatformParams: function () { return r.platformParams }, getEventCallConfigParams: function () { var f = r, a = f.modStatsParams, b = f.platformParams, d; if (!a) { a = {}; for (d in b) b.hasOwnProperty(d) && !f.nonModStatsParams[d] && (a[d.replace(/^d_/, "")] = b[d]); f.modStatsParams = a } return a }
    }, A = {
        corsMetadata: function () {
            var f = "none", a = !0; "undefined" !== typeof XMLHttpRequest && XMLHttpRequest === Object(XMLHttpRequest) && ("withCredentials" in new XMLHttpRequest ? f = "XMLHttpRequest" : (new Function("/*@cc_on return /^10/.test(@_jscript_version) @*/"))() ?
            f = "XMLHttpRequest" : "undefined" !== typeof XDomainRequest && XDomainRequest === Object(XDomainRequest) && (a = !1), 0 < Object.prototype.toString.call(window.HTMLElement).indexOf("Constructor") && (a = !1)); return { corsType: f, corsCookiesEnabled: a }
        }(), getCORSInstance: function () { return "none" === this.corsMetadata.corsType ? null : new window[this.corsMetadata.corsType] }, submitRequest: function (f) { r.registerRequest(A.createQueuedRequest(f)); return !0 }, createQueuedRequest: function (f) {
            var a = r, b, d = f.callback, c = "img", g; if (!v.isEmptyObject(n)) {
                var e,
                m, l; for (e in n) n.hasOwnProperty(e) && (m = n[e], null != m && "" !== m && e in f && !(m in f || m in r.reservedKeys) && (l = f[e], null != l && "" !== l && (f[m] = l)))
            } v.isValidPdata(f.sids) || (f.sids = []); v.isValidPdata(f.pdata) || (f.pdata = []); v.isValidLogdata(f.logdata) || (f.logdata = {}); f.logdataArray = t.convertObjectToKeyValuePairs(f.logdata, "=", !0); f.logdataArray.push("_ts=" + (new Date).getTime()); "function" !== typeof d && (d = this.defaultCallback); a.useJSONP = !0 !== f.useImageRequest; a.useJSONP && (c = "script", b = a.callbackPrefix + "_" + h + "_" +
            (new Date).getTime()); a = this.makeRequestSrcData(f, b); !P && (g = this.getCORSInstance()) && a.truncated && (this.corsMetadata.corsCookiesEnabled || a.isDeclaredIdCall) && (c = "cors"); return { tag: c, src: a.src, corsSrc: a.corsSrc, internalCallbackName: b, callbackFn: d, postCallbackFn: f.postCallbackFn, useImageRequest: !!f.useImageRequest, requestData: f, corsInstance: g, corsPostData: a.corsPostData, hasCORSError: !1 }
        }, defaultCallback: function (f, a) {
            var b, d, c, e, g, h, m, n, w; if ((b = f.stuff) && b instanceof Array && (d = b.length)) for (c = 0; c < d; c++) if ((e =
            b[c]) && e === Object(e)) { g = e.cn; h = e.cv; m = e.ttl; if ("undefined" === typeof m || "" === m) m = Math.floor(t.getMaxCookieExpiresInMinutes() / 60 / 24); n = e.dmn || "." + document.domain.replace(/^www\./, ""); w = e.type; g && (h || "number" === typeof h) && ("var" !== w && (m = parseInt(m, 10)) && !isNaN(m) && t.setCookie(g, h, 1440 * m, "/", n, !1), G.stuffed[g] = h) } b = f.uuid; v.isPopulatedString(b) && !v.isEmptyObject(l) && (d = l.path, "string" === typeof d && d.length || (d = "/"), c = parseInt(l.days, 10), isNaN(c) && (c = 100), t.setCookie(l.name || "aam_did", b, 1440 * c, d, l.domain ||
            "." + document.domain.replace(/^www\./, ""), !0 === l.secure)); q || r.abortRequests || z.requestToProcess(f, a)
        }, makeRequestSrcData: function (f, a) {
            f.sids = v.removeEmptyArrayValues(f.sids || []); f.pdata = v.removeEmptyArrayValues(f.pdata || []); var b = r, d = b.platformParams, c = t.encodeAndBuildRequest(f.sids, ","), g = t.encodeAndBuildRequest(f.pdata, ","), m = (f.logdataArray || []).join("&"); delete f.logdataArray; var n = y.IS_HTTPS ? "https://" : "http://", l = b.declaredId.getDeclaredIdQueryString(), k; k = []; var w, q, p, u; for (w in f) if (!(w in
            b.reservedKeys) && f.hasOwnProperty(w)) if (q = f[w], w = encodeURIComponent(w), q instanceof Array) for (p = 0, u = q.length; p < u; p++) k.push(w + "=" + encodeURIComponent(q[p])); else k.push(w + "=" + encodeURIComponent(q)); k = k.length ? "&" + k.join("&") : ""; w = !1; c = "d_nsid=" + d.d_nsid + l + (c.length ? "&d_sid=" + c : "") + (g.length ? "&d_px=" + g : "") + (m.length ? "&d_ld=" + encodeURIComponent(m) : ""); d = "&d_rtbd=" + d.d_rtbd + "&d_jsonv=" + d.d_jsonv + "&d_dst=" + d.d_dst; n = n + e + ".demdex.net/event"; g = b = n + "?" + c + (b.useJSONP ? d + "&d_cb=" + (a || "") : "") + k; 2048 < b.length &&
            (b = b.substring(0, b.lastIndexOf("&")), w = !0); return { corsSrc: n + "?" + (O ? "testcors=1&d_nsid=" + h + "&" : "") + "_ts=" + (new Date).getTime(), src: b, originalSrc: g, truncated: w, corsPostData: c + d + k, isDeclaredIdCall: "" !== l }
        }, fireRequest: function (f) { if ("img" === f.tag) this.fireImage(f); else { var a = r.declaredId, a = a.declaredId.request || a.declaredId.init || {}, a = { dpid: a.dpid || "", dpuuid: a.dpuuid || "" }; "script" === f.tag ? this.fireScript(f, a) : "cors" === f.tag && this.fireCORS(f, a) } }, fireImage: function (a) {
            var c = r, e, g; c.abortRequests || (c.firing =
            !0, e = new Image(0, 0), c.sent.push(a), e.onload = function () { c.firing = !1; c.fired.push(a); c.num_of_img_responses++; c.registerRequest() }, g = function (e) { b = "imgAbortOrErrorHandler received the event of type " + e.type; d.push(b); c.abortRequests = !0; c.firing = !1; c.errored.push(a); c.num_of_img_errors++; c.registerRequest() }, e.addEventListener ? (e.addEventListener("error", g, !1), e.addEventListener("abort", g, !1)) : e.attachEvent && (e.attachEvent("onerror", g), e.attachEvent("onabort", g)), e.src = a.src)
        }, fireScript: function (a,
        c) {
            var g = this, h = r, m, n, l = a.src, k = a.postCallbackFn, q = "function" === typeof k, p = a.internalCallbackName; h.abortRequests || (h.firing = !0, window[p] = function (g) {
                try { g !== Object(g) && (g = {}); var m = a.callbackFn; h.firing = !1; h.fired.push(a); h.num_of_jsonp_responses++; m(g, c); q && k(g, c) } catch (l) {
                    l.message = "DIL jsonp callback caught error with message " + l.message; b = l.message; d.push(b); l.filename = l.filename || "dil.js"; l.partner = e; DIL.errorModule.handleError(l); try {
                        m({ error: l.name + "|" + l.message }, c), q && k({
                            error: l.name + "|" +
                            l.message
                        }, c)
                    } catch (H) { }
                } finally { h.requestRemoval({ script: n, callbackName: p }), h.registerRequest() }
            }, L ? (h.firing = !1, h.requestRemoval({ script: "no script created", callbackName: p })) : (n = document.createElement("script"), n.addEventListener && n.addEventListener("error", function (d) { h.requestRemoval({ script: n, callbackName: p }); b = "jsonp script tag error listener received the event of type " + d.type + " with src " + l; g.handleScriptError(b, a) }, !1), n.type = "text/javascript", n.src = l, m = DIL.variables.scriptNodeList[0], m.parentNode.insertBefore(n,
            m)), h.sent.push(a), h.declaredId.declaredId.request = null)
        }, fireCORS: function (a, c) {
            function g(n) {
                var l; try { if (l = JSON.parse(n), l !== Object(l)) { h.handleCORSError(a, c, "Response is not JSON"); return } } catch (k) { h.handleCORSError(a, c, "Error parsing response as JSON"); return } try { var H = a.callbackFn; m.firing = !1; m.fired.push(a); m.num_of_cors_responses++; H(l, c); t && q(l, c) } catch (p) {
                    p.message = "DIL handleCORSResponse caught error with message " + p.message; b = p.message; d.push(b); p.filename = p.filename || "dil.js"; p.partner =
                    e; DIL.errorModule.handleError(p); try { H({ error: p.name + "|" + p.message }, c), t && q({ error: p.name + "|" + p.message }, c) } catch (r) { }
                } finally { m.registerRequest() }
            } var h = this, m = r, n = this.corsMetadata.corsType, l = a.corsSrc, k = a.corsInstance, p = a.corsPostData, q = a.postCallbackFn, t = "function" === typeof q; if (!m.abortRequests) {
                m.firing = !0; if (M) m.firing = !1; else try {
                    k.open("post", l, !0), "XMLHttpRequest" === n ? (k.withCredentials = !0, k.setRequestHeader("Content-Type", "application/x-www-form-urlencoded"), k.onreadystatechange = function () {
                        4 ===
                        this.readyState && (200 === this.status ? g(this.responseText) : h.handleCORSError(a, c, "onreadystatechange"))
                    }) : "XDomainRequest" === n && (k.onload = function () { g(this.responseText) }), k.onerror = function () { h.handleCORSError(a, c, "onerror") }, k.ontimeout = function () { h.handleCORSError(a, c, "ontimeout") }, k.send(p)
                } catch (u) { this.handleCORSError(a, c, "try-catch") } m.sent.push(a); m.declaredId.declaredId.request = null
            }
        }, handleCORSError: function (a, b, c) {
            a.hasCORSError || (a.hasCORSError = !0, r.num_of_cors_errors++, r.corsErrorSources.push(c),
            a.tag = "script", this.fireScript(a, b))
        }, handleScriptError: function (a, b) { r.num_of_jsonp_errors++; this.handleRequestError(a, b) }, handleRequestError: function (a, b) { var c = r; d.push(a); c.abortRequests = !0; c.firing = !1; c.errored.push(b); c.registerRequest() }
    }, v = {
        isValidPdata: function (a) { return a instanceof Array && this.removeEmptyArrayValues(a).length ? !0 : !1 }, isValidLogdata: function (a) { return !this.isEmptyObject(a) }, isEmptyObject: function (a) {
            if (a !== Object(a)) return !0; for (var b in a) if (a.hasOwnProperty(b)) return !1;
            return !0
        }, removeEmptyArrayValues: function (a) { for (var b = 0, c = a.length, d, g = [], b = 0; b < c; b++) d = a[b], "undefined" !== typeof d && null !== d && "" !== d && g.push(d); return g }, isPopulatedString: function (a) { return "string" === typeof a && a.length }
    }, t = {
        addListener: function () { if (document.addEventListener) return function (a, b, c) { a.addEventListener(b, function (a) { "function" === typeof c && c(a) }, !1) }; if (document.attachEvent) return function (a, b, c) { a.attachEvent("on" + b, function (a) { "function" === typeof c && c(a) }) } }(), convertObjectToKeyValuePairs: function (a,
        b, c) { var d = [], g, e; b || (b = "="); for (g in a) a.hasOwnProperty(g) && (e = a[g], "undefined" !== typeof e && null !== e && "" !== e && d.push(g + b + (c ? encodeURIComponent(e) : e))); return d }, encodeAndBuildRequest: function (a, b) { return this.map(a, function (a) { return encodeURIComponent(a) }).join(b) }, map: function (a, b) {
            if (Array.prototype.map) return a.map(b); if (void 0 === a || null === a) throw new TypeError; var c = Object(a), d = c.length >>> 0; if ("function" !== typeof b) throw new TypeError; for (var g = Array(d), e = 0; e < d; e++) e in c && (g[e] = b.call(b, c[e],
            e, c)); return g
        }, filter: function (a, b) { if (!Array.prototype.filter) { if (void 0 === a || null === a) throw new TypeError; var c = Object(a), d = c.length >>> 0; if ("function" !== typeof b) throw new TypeError; for (var g = [], e = 0; e < d; e++) if (e in c) { var h = c[e]; b.call(b, h, e, c) && g.push(h) } return g } return a.filter(b) }, getCookie: function (a) {
            a += "="; var b = document.cookie.split(";"), c, d, e; c = 0; for (d = b.length; c < d; c++) {
                for (e = b[c]; " " === e.charAt(0) ;) e = e.substring(1, e.length); if (0 === e.indexOf(a)) return decodeURIComponent(e.substring(a.length,
                e.length))
            } return null
        }, setCookie: function (a, b, c, d, e, g) { var h = new Date; c && (c *= 6E4); document.cookie = a + "=" + encodeURIComponent(b) + (c ? ";expires=" + (new Date(h.getTime() + c)).toUTCString() : "") + (d ? ";path=" + d : "") + (e ? ";domain=" + e : "") + (g ? ";secure" : "") }, extendArray: function (a, b) { return a instanceof Array && b instanceof Array ? (Array.prototype.push.apply(a, b), !0) : !1 }, extendObject: function (a, b, c) { var d; if (a === Object(a) && b === Object(b)) { for (d in b) !b.hasOwnProperty(d) || !v.isEmptyObject(c) && d in c || (a[d] = b[d]); return !0 } return !1 },
        getMaxCookieExpiresInMinutes: function () { return ((new Date(y.COOKIE_MAX_EXPIRATION_DATE)).getTime() - (new Date).getTime()) / 1E3 / 60 }
    }; "error" === e && 0 === h && t.addListener(window, "load", function () { DIL.windowLoaded = !0 }); var B = function () { r.registerRequest(); S(); q || r.abortRequests || z.attachIframe(); r.readyToRemove = !0; r.requestRemoval() }, S = function () {
        q || setTimeout(function () { N || r.firstRequestHasFired || r.adms.admsProcessingStarted || r.adms.calledBack || ("function" === typeof F ? I.afterResult(F).submit() : I.submit()) },
        DIL.constants.TIME_TO_DEFAULT_REQUEST)
    }, R = document; "error" !== e && (DIL.windowLoaded ? B() : "complete" !== R.readyState && "loaded" !== R.readyState ? t.addListener(window, "load", B) : DIL.isAddedPostWindowLoadWasCalled ? t.addListener(window, "load", B) : E || (k = "number" === typeof k ? parseInt(k, 10) : 0, 0 > k && (k = 0), setTimeout(B, k || DIL.constants.TIME_TO_CATCH_ALL_DP_IFRAME_ATTACHMENT))); r.declaredId.setDeclaredId(J, "init"); this.api = I; this.getStuffedVariable = function (a) {
        var b = G.stuffed[a]; b || "number" === typeof b || (b = t.getCookie(a)) ||
        "number" === typeof b || (b = ""); return b
    }; this.validators = v; this.helpers = t; this.constants = y; this.log = d; Q && (this.pendingRequest = u, this.requestController = r, this.setDestinationPublishingUrl = g, this.destinationPublishing = z, this.requestProcs = A, this.variables = G, this.callWindowLoadFunctions = B)
}, function () { var a = document, c; null == a.readyState && a.addEventListener && (a.readyState = "loading", a.addEventListener("DOMContentLoaded", c = function () { a.removeEventListener("DOMContentLoaded", c, !1); a.readyState = "complete" }, !1)) }(),
DIL.extendStaticPropertiesAndMethods = function (a) { var c; if (a === Object(a)) for (c in a) a.hasOwnProperty(c) && (this[c] = a[c]) }, DIL.extendStaticPropertiesAndMethods({
    version: "5.7", jsonVersion: 1, constants: { TIME_TO_DEFAULT_REQUEST: 50, TIME_TO_CATCH_ALL_DP_IFRAME_ATTACHMENT: 500 }, variables: { scriptNodeList: document.getElementsByTagName("script"), scriptsRemoved: [], callbacksRemoved: [] }, windowLoaded: !1, dils: {}, isAddedPostWindowLoadWasCalled: !1, isAddedPostWindowLoad: function (a) {
        this.isAddedPostWindowLoadWasCalled =
        !0; this.windowLoaded = "function" === typeof a ? !!a() : "boolean" === typeof a ? a : !0
    }, create: function (a) { try { return new DIL(a) } catch (c) { return (new Image(0, 0)).src = "http://error.demdex.net/event?d_nsid=0&d_px=14137&d_ld=name%3Derror%26filename%3Ddil.js%26partner%3Dno_partner%26message%3DError%2520in%2520attempt%2520to%2520create%2520DIL%2520instance%2520with%2520DIL.create()%26_ts%3D" + (new Date).getTime(), Error("Error in attempt to create DIL instance with DIL.create()") } }, registerDil: function (a, c, d) {
        c = c + "$" +
        d; c in this.dils || (this.dils[c] = a)
    }, getDil: function (a, c) { var d; "string" !== typeof a && (a = ""); c || (c = 0); d = a + "$" + c; return d in this.dils ? this.dils[d] : Error("The DIL instance with partner = " + a + " and containerNSID = " + c + " was not found") }, dexGetQSVars: function (a, c, d) { c = this.getDil(c, d); return c instanceof this ? c.getStuffedVariable(a) : "" }, xd: {
        postMessage: function (a, c, d) {
            var b = 1; c && (window.postMessage ? d.postMessage(a, c.replace(/([^:]+:\/\/[^\/]+).*/, "$1")) : c && (d.location = c.replace(/#.*$/, "") + "#" + +new Date +
            b++ + "&" + a))
        }
    }
}), DIL.errorModule = function () {
    var a = DIL.create({ partner: "error", containerNSID: 0, disableDestinationPublishingIframe: !0 }), c = { harvestererror: 14138, destpuberror: 14139, dpmerror: 14140, generalerror: 14137, error: 14137, noerrortypedefined: 15021, evalerror: 15016, rangeerror: 15017, referenceerror: 15018, typeerror: 15019, urierror: 15020 }, d = !1; return {
        activate: function () { d = !0 }, handleError: function (b) {
            if (!d) return "DIL error module has not been activated"; b !== Object(b) && (b = {}); var g = b.name ? (b.name + "").toLowerCase() :
            "", e = []; b = { name: g, filename: b.filename ? b.filename + "" : "", partner: b.partner ? b.partner + "" : "no_partner", site: b.site ? b.site + "" : document.location.href, message: b.message ? b.message + "" : "" }; e.push(g in c ? c[g] : c.noerrortypedefined); a.api.pixels(e).logs(b).useImageRequest().submit(); return "DIL error report sent"
        }, pixelMap: c
    }
}(), DIL.tools = {}, DIL.modules = {
    helpers: {
        handleModuleError: function (a, c, d) {
            var b = ""; c = c || "Error caught in DIL module/submodule: "; a === Object(a) ? b = c + (a.message || "err has no message") : (b = c + "err is not a valid object",
            a = {}); a.message = b; d instanceof DIL && (a.partner = d.api.getPartner()); DIL.errorModule.handleError(a); return this.errorMessage = b
        }
    }
});
DIL.tools.getSearchReferrer = function (a, c) {
    var d = DIL.getDil("error"), b = DIL.tools.decomposeURI(a || document.referrer), g = "", e = "", h = { queryParam: "q" }; return (g = d.helpers.filter([c === Object(c) ? c : {}, { hostPattern: /aol\./ }, { hostPattern: /ask\./ }, { hostPattern: /bing\./ }, { hostPattern: /google\./ }, { hostPattern: /yahoo\./, queryParam: "p" }], function (a) { return !(!a.hasOwnProperty("hostPattern") || !b.hostname.match(a.hostPattern)) }).shift()) ? {
        valid: !0, name: b.hostname, keywords: (d.helpers.extendObject(h, g), e = h.queryPattern ?
        (g = ("" + b.search).match(h.queryPattern)) ? g[1] : "" : b.uriParams[h.queryParam], decodeURIComponent(e || "").replace(/\+|%20/g, " "))
    } : { valid: !1, name: "", keywords: "" }
};
DIL.tools.decomposeURI = function (a) { var c = DIL.getDil("error"), d = document.createElement("a"); d.href = a || document.referrer; return { hash: d.hash, host: d.host.split(":").shift(), hostname: d.hostname, href: d.href, pathname: d.pathname.replace(/^\//, ""), protocol: d.protocol, search: d.search, uriParams: function (a, d) { c.helpers.map(d.split("&"), function (c) { c = c.split("="); a[c.shift()] = c.shift() }); return a }({}, d.search.replace(/^(\/|\?)?|\/$/g, "")) } };
DIL.tools.getMetaTags = function () { var a = {}, c = document.getElementsByTagName("meta"), d, b, g, e, h; d = 0; for (g = arguments.length; d < g; d++) if (e = arguments[d], null !== e) for (b = 0; b < c.length; b++) if (h = c[b], h.name === e) { a[e] = h.content; break } return a };
DIL.modules.siteCatalyst = {
    dil: null, handle: DIL.modules.helpers.handleModuleError, init: function (a, c, d, b) {
        try {
            var g = this, e = { name: "DIL Site Catalyst Module Error" }, h = function (a) { e.message = a; DIL.errorModule.handleError(e); return a }; this.options = b === Object(b) ? b : {}; this.dil = null; if (c instanceof DIL) this.dil = c; else return h("dilInstance is not a valid instance of DIL"); e.partner = c.api.getPartner(); if (a !== Object(a)) return h("siteCatalystReportingSuite is not an object"); window.AppMeasurement_Module_DIL = a.m_DIL =
            function (a) {
                var b = "function" === typeof a.m_i ? a.m_i("DIL") : this; if (b !== Object(b)) return h("m is not an object"); b.trackVars = g.constructTrackVars(d); b.d = 0; b.s = a; b._t = function () {
                    var a, b, c = "," + this.trackVars + ",", d = this.s, e, k = []; e = []; var p = {}, q = !1; if (d !== Object(d)) return h("Error in m._t function: s is not an object"); if (this.d) {
                        if ("function" === typeof d.foreachVar) d.foreachVar(function (a, b) { "undefined" !== typeof b && (p[a] = b, q = !0) }, this.trackVars); else {
                            if (!(d.va_t instanceof Array)) return h("Error in m._t function: s.va_t is not an array");
                            if (d.lightProfileID) (a = d.lightTrackVars) && (a = "," + a + "," + d.vl_mr + ","); else if (d.pe || d.linkType) a = d.linkTrackVars, d.pe && (b = d.pe.substring(0, 1).toUpperCase() + d.pe.substring(1), d[b] && (a = d[b].trackVars)), a && (a = "," + a + "," + d.vl_l + "," + d.vl_l2 + ","); if (a) { b = 0; for (k = a.split(",") ; b < k.length; b++) 0 <= c.indexOf("," + k[b] + ",") && e.push(k[b]); e.length && (c = "," + e.join(",") + ",") } e = 0; for (b = d.va_t.length; e < b; e++) a = d.va_t[e], 0 <= c.indexOf("," + a + ",") && "undefined" !== typeof d[a] && null !== d[a] && "" !== d[a] && (p[a] = d[a], q = !0)
                        } g.includeContextData(d,
                        p).store_populated && (q = !0); q && this.d.api.signals(p, "c_").submit()
                    }
                }
            }; a.loadModule("DIL"); a.DIL.d = c; return e.message ? e.message : "DIL.modules.siteCatalyst.init() completed with no errors"
        } catch (k) { return this.handle(k, "DIL.modules.siteCatalyst.init() caught error with message ", this.dil) }
    }, constructTrackVars: function (a) {
        var c = [], d, b, g, e, h; if (a === Object(a)) {
            d = a.names; if (d instanceof Array && (g = d.length)) for (b = 0; b < g; b++) e = d[b], "string" === typeof e && e.length && c.push(e); a = a.iteratedNames; if (a instanceof Array &&
            (g = a.length)) for (b = 0; b < g; b++) if (d = a[b], d === Object(d) && (e = d.name, h = parseInt(d.maxIndex, 10), "string" === typeof e && e.length && !isNaN(h) && 0 <= h)) for (d = 0; d <= h; d++) c.push(e + d); if (c.length) return c.join(",")
        } return this.constructTrackVars({ names: "pageName channel campaign products events pe pev1 pev2 pev3".split(" "), iteratedNames: [{ name: "prop", maxIndex: 75 }, { name: "eVar", maxIndex: 250 }] })
    }, includeContextData: function (a, c) {
        var d = {}, b = !1; if (a.contextData === Object(a.contextData)) {
            var g = a.contextData, e = this.options.replaceContextDataPeriodsWith,
            h = this.options.filterFromContextVariables, k = {}, q, p, n, l; "string" === typeof e && e.length || (e = "_"); if (h instanceof Array) for (q = 0, p = h.length; q < p; q++) n = h[q], this.dil.validators.isPopulatedString(n) && (k[n] = !0); for (l in g) !g.hasOwnProperty(l) || k[l] || !(h = g[l]) && "number" !== typeof h || (l = ("contextData." + l).replace(/\./g, e), c[l] = h, b = !0)
        } d.store_populated = b; return d
    }
};
DIL.modules.GA = {
    dil: null, arr: null, tv: null, errorMessage: "", defaultTrackVars: ["_setAccount", "_setCustomVar", "_addItem", "_addTrans", "_trackSocial"], defaultTrackVarsObj: null, signals: {}, hasSignals: !1, handle: DIL.modules.helpers.handleModuleError, init: function (a, c, d) {
        try {
            this.tv = this.arr = this.dil = null; this.errorMessage = ""; this.signals = {}; this.hasSignals = !1; var b = { name: "DIL GA Module Error" }, g = ""; c instanceof DIL ? (this.dil = c, b.partner = this.dil.api.getPartner()) : (g = "dilInstance is not a valid instance of DIL",
            b.message = g, DIL.errorModule.handleError(b)); a instanceof Array && a.length ? this.arr = a : (g = "gaArray is not an array or is empty", b.message = g, DIL.errorModule.handleError(b)); this.tv = this.constructTrackVars(d); this.errorMessage = g
        } catch (e) { this.handle(e, "DIL.modules.GA.init() caught error with message ", this.dil) } finally { return this }
    }, constructTrackVars: function (a) {
        var c = [], d, b, g, e; if (this.defaultTrackVarsObj !== Object(this.defaultTrackVarsObj)) {
            g = this.defaultTrackVars; e = {}; d = 0; for (b = g.length; d < b; d++) e[g[d]] =
            !0; this.defaultTrackVarsObj = e
        } else e = this.defaultTrackVarsObj; if (a === Object(a)) { a = a.names; if (a instanceof Array && (b = a.length)) for (d = 0; d < b; d++) g = a[d], "string" === typeof g && g.length && g in e && c.push(g); if (c.length) return c } return this.defaultTrackVars
    }, constructGAObj: function (a) {
        var c = {}; a = a instanceof Array ? a : this.arr; var d, b, g, e; d = 0; for (b = a.length; d < b; d++) g = a[d], g instanceof Array && g.length && (g = [], e = a[d], g instanceof Array && e instanceof Array && Array.prototype.push.apply(g, e), e = g.shift(), "string" ===
        typeof e && e.length && (c[e] instanceof Array || (c[e] = []), c[e].push(g))); return c
    }, addToSignals: function (a, c) { if ("string" !== typeof a || "" === a || null == c || "" === c) return !1; this.signals[a] instanceof Array || (this.signals[a] = []); this.signals[a].push(c); return this.hasSignals = !0 }, constructSignals: function () {
        var a = this.constructGAObj(), c = {
            _setAccount: function (a) { this.addToSignals("c_accountId", a) }, _setCustomVar: function (a, b, c) { "string" === typeof b && b.length && this.addToSignals("c_" + b, c) }, _addItem: function (a, b, c, d,
            e, g) { this.addToSignals("c_itemOrderId", a); this.addToSignals("c_itemSku", b); this.addToSignals("c_itemName", c); this.addToSignals("c_itemCategory", d); this.addToSignals("c_itemPrice", e); this.addToSignals("c_itemQuantity", g) }, _addTrans: function (a, b, c, d, e, g, h, k) {
                this.addToSignals("c_transOrderId", a); this.addToSignals("c_transAffiliation", b); this.addToSignals("c_transTotal", c); this.addToSignals("c_transTax", d); this.addToSignals("c_transShipping", e); this.addToSignals("c_transCity", g); this.addToSignals("c_transState",
                h); this.addToSignals("c_transCountry", k)
            }, _trackSocial: function (a, b, c, d) { this.addToSignals("c_socialNetwork", a); this.addToSignals("c_socialAction", b); this.addToSignals("c_socialTarget", c); this.addToSignals("c_socialPagePath", d) }
        }, d = this.tv, b, g, e, h, k, q; b = 0; for (g = d.length; b < g; b++) if (e = d[b], a.hasOwnProperty(e) && c.hasOwnProperty(e) && (q = a[e], q instanceof Array)) for (h = 0, k = q.length; h < k; h++) c[e].apply(this, q[h])
    }, submit: function () {
        try {
            if ("" !== this.errorMessage) return this.errorMessage; this.constructSignals();
            return this.hasSignals ? (this.dil.api.signals(this.signals).submit(), "Signals sent: " + this.dil.helpers.convertObjectToKeyValuePairs(this.signals, "=", !0) + this.dil.log) : "No signals present"
        } catch (a) { return this.handle(a, "DIL.modules.GA.submit() caught error with message ", this.dil) }
    }, Stuffer: {
        LIMIT: 5, dil: null, cookieName: null, delimiter: null, errorMessage: "", handle: DIL.modules.helpers.handleModuleError, callback: null, v: function () { return !1 }, init: function (a, c, d) {
            try {
                this.callback = this.dil = null, this.errorMessage =
                "", a instanceof DIL ? (this.dil = a, this.v = this.dil.validators.isPopulatedString, this.cookieName = this.v(c) ? c : "aam_ga", this.delimiter = this.v(d) ? d : "|") : this.handle({ message: "dilInstance is not a valid instance of DIL" }, "DIL.modules.GA.Stuffer.init() error: ")
            } catch (b) { this.handle(b, "DIL.modules.GA.Stuffer.init() caught error with message ", this.dil) } finally { return this }
        }, process: function (a) {
            var c, d, b, g, e, h; h = !1; var k = 1; if (a === Object(a) && (c = a.stuff) && c instanceof Array && (d = c.length)) for (a = 0; a < d; a++) if ((b =
            c[a]) && b === Object(b) && (g = b.cn, e = b.cv, g === this.cookieName && this.v(e))) { h = !0; break } if (h) { c = e.split(this.delimiter); "undefined" === typeof window._gaq && (window._gaq = []); b = window._gaq; a = 0; for (d = c.length; a < d && !(h = c[a].split("="), e = h[0], h = h[1], this.v(e) && this.v(h) && b.push(["_setCustomVar", k++, e, h, 1]), k > this.LIMIT) ; a++); this.errorMessage = 1 < k ? "No errors - stuffing successful" : "No valid values to stuff" } else this.errorMessage = "Cookie name and value not found in json"; if ("function" === typeof this.callback) return this.callback()
        },
        submit: function () { try { var a = this; if ("" !== this.errorMessage) return this.errorMessage; this.dil.api.afterResult(function (c) { a.process(c) }).submit(); return "DIL.modules.GA.Stuffer.submit() successful" } catch (c) { return this.handle(c, "DIL.modules.GA.Stuffer.submit() caught error with message ", this.dil) } }
    }
};
DIL.modules.Peer39 = {
    aid: "", dil: null, optionals: null, errorMessage: "", calledBack: !1, script: null, scriptsSent: [], returnedData: [], handle: DIL.modules.helpers.handleModuleError, init: function (a, c, d) {
        try {
            this.dil = null; this.errorMessage = ""; this.calledBack = !1; this.optionals = d === Object(d) ? d : {}; d = { name: "DIL Peer39 Module Error" }; var b = [], g = ""; this.isSecurePageButNotEnabled(document.location.protocol) && (g = "Module has not been enabled for a secure page", b.push(g), d.message = g, DIL.errorModule.handleError(d)); c instanceof
            DIL ? (this.dil = c, d.partner = this.dil.api.getPartner()) : (g = "dilInstance is not a valid instance of DIL", b.push(g), d.message = g, DIL.errorModule.handleError(d)); "string" === typeof a && a.length ? this.aid = a : (g = "aid is not a string or is empty", b.push(g), d.message = g, DIL.errorModule.handleError(d)); this.errorMessage = b.join("\n")
        } catch (e) { this.handle(e, "DIL.modules.Peer39.init() caught error with message ", this.dil) } finally { return this }
    }, isSecurePageButNotEnabled: function (a) {
        return "https:" === a && !0 !== this.optionals.enableHTTPS ?
        !0 : !1
    }, constructSignals: function () { var a = this, c = this.constructScript(), d = DIL.variables.scriptNodeList[0]; window["afterFinished_" + this.aid] = function () { try { var b = a.processData(p39_KVP_Short("c_p", "|").split("|")); b.hasSignals && a.dil.api.signals(b.signals).submit() } catch (c) { } finally { a.calledBack = !0, "function" === typeof a.optionals.afterResult && a.optionals.afterResult() } }; d.parentNode.insertBefore(c, d); this.scriptsSent.push(c); return "Request sent to Peer39" }, processData: function (a) {
        var c, d, b, g, e = {}, h =
        !1; this.returnedData.push(a); if (a instanceof Array) for (c = 0, d = a.length; c < d; c++) b = a[c].split("="), g = b[0], b = b[1], g && isFinite(b) && !isNaN(parseInt(b, 10)) && (e[g] instanceof Array || (e[g] = []), e[g].push(b), h = !0); return { hasSignals: h, signals: e }
    }, constructScript: function () {
        var a = document.createElement("script"), c = this.optionals, d = c.scriptId, b = c.scriptSrc, c = c.scriptParams; a.id = "string" === typeof d && d.length ? d : "peer39ScriptLoader"; a.type = "text/javascript"; "string" === typeof b && b.length ? a.src = b : (a.src = (this.dil.constants.IS_HTTPS ?
        "https:" : "http:") + "//stags.peer39.net/" + this.aid + "/trg_" + this.aid + ".js", "string" === typeof c && c.length && (a.src += "?" + c)); return a
    }, submit: function () { try { return "" !== this.errorMessage ? this.errorMessage : this.constructSignals() } catch (a) { return this.handle(a, "DIL.modules.Peer39.submit() caught error with message ", this.dil) } }
};

//Fetching at_cid cookie value
var c_name = "at_cid";
function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");

    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}
var idSync = getCookie(c_name);


/*Defensive checking to make sure the ID for Declared ID is present in traderca*/

if (typeof idSync != 'undefined' && idSync != "") {
    var cGuid = idSync
    var objP = {
        partner: 'traderca',
        uuidCookie: {
            name: 'aam_uuid',
            days: 30
        },
        declaredId: {
            dpid: "17102",
            dpuuid: cGuid
        }
    };
} else {
    /*if the ID isn’t present,  create the object without the declared id*/
    var objP = {
        partner: 'traderca',
        uuidCookie: {
            name: 'aam_uuid',
            days: 30
        }
    };
};

/*now that we have configured the object of the argument, create my DIL instance*/
var ATDilInstance = DIL.create(objP);

//Make sure to update the object name as required
//For example below we are passing a key value pair in customObj

//Check for presence of customObj object to avoid throwing errors
if (typeof customObj != 'undefined') {
    //Object is present, submit data
    var ATDilInstance = DIL.getDil('traderca');
    ATDilInstance.api.signals(customObj, 'c_').submit();
};

// End DIL Code
try {
    _gaq.push(function () {
        jQuery(document).ready(function () {
            var pageTracker = _gat._getTrackerByName(),
                href = '';
            jQuery('a').each(function () {
                href = jQuery(this).attr('href');
                if (typeof href == 'string' && href.search(wa_xdomainlist) > -1 && (href.search(wa_domain) == -1 || (href.search(wa_domain) > -1 && href.search(wa_xsubdomainlist) > -1))) {
                    jQuery(this).attr('href', pageTracker._getLinkerUrl(href, true));
                }
            });
        });
    });
} catch (e) { } (function () {
    var ga = document.createElement('script');
    ga.type = 'text/javascript';
    ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'stats.g.doubleclick.net/dc.js';
    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(ga, s);
})();
_cpga.trackSearch();