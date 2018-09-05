'use strict'

function YesNoPicker($uibModalInstance, title, description, name) {
    var $item = this;
    $item.Title = title;
    $item.Description = description;
    $item.Name = name;
    $item.ok = function () {
        $uibModalInstance.close({
            name: $item.Name
        });
    };

    $item.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};