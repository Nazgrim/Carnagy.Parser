var garajModule = (function () {

    function parentSearch(categories, parentId, currentId) {
        var currentParentId = -1;
        for (var i = 0; i < categories.length; i++) {
            if (categories[i].id == currentId) {
                currentParentId = categories[i].parentId
                break;
            }
        }
        if (currentParentId == parentId) {
            return true;
        }
        else if (currentParentId == 0) {
            return false;
        }
        else {
            return parentSearch(categories, parentId, currentParentId);
        }
    }

    return {
        parentSearch: function (categories, parentId, category) {
            return parentSearch(categories, parentId, category);
        },
        getDealers: function (cars, categories, parentId) {
            var cars2 = cars.filter(function (car, index) {
                return parentSearch(categories, parentId, car.category);
            });
            return cars2;
        },
        createGropus: function (cars, filters) {
            var result = {}
            cars.forEach(function (car, index) {
                var keyArray = [];
                var keyObject = {};
                filters.forEach(function (filter) {
                    keyObject[filter.field] = car[filter.field];
                    keyArray.push(car[filter.field]);
                });
                var ketString = keyArray.join('.');
                if (!result[ketString]) {
                    result[ketString] = { key: keyObject }
                }
                if (!result[ketString]["value"])
                    result[ketString]["value"] = [];
                result[ketString]["value"].push(car);
            });
            return result;
        },

    }
} ());