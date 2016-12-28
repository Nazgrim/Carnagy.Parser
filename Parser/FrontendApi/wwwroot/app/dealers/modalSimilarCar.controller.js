class ModalSimilarCarCtrl {
    constructor($scope, $uibModalInstance, filterFilter, items, carService) {
        $scope.items = items;
        $scope.ok = function () {
            $uibModalInstance.close(filterFilter($scope.items, { checked: true }));
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
}
ModalSimilarCarCtrl.$inject = ['$scope', '$uibModalInstance', 'filterFilter', 'items', 'carService'];

export default ModalSimilarCarCtrl;