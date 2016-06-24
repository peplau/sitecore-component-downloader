var componentDownloaderDialog;

define(["sitecore"], function (Sitecore) {
    var ListPageDialog = Sitecore.Definitions.App.extend({
        initialized: function() {
            componentDownloaderDialog = this;

            // Expand all nodes
            //this.expandAllTreeViews();
            //window.setTimeout(function () {
            //componentDownloaderDialog.collapseAllTreeViews();
            //window.scrollTo(0, 0);

            this.hideLoading();
            //}, 1500);
        },
        showLoading: function() {
            $("[data-sc-id='SectionMain']").hide();
            $("[data-sc-id='LabLoading']").show();
        },
        hideLoading: function () {
            $("[data-sc-id='SectionMain']").show();
            $("[data-sc-id='LabLoading']").hide();
        },
        getAllTreeViews: function () {
            return new Array(
                this.TreeDsTemplate,
                this.TreeDsBaseTemplates,
                this.TreeDsItems,
                this.TreePlaceholderSettings,
                this.TreeRules,
                this.TreeExpEditorButtons,
                this.TreeParametersTemplate,
                this.TreeThumbnail
            );
        },
        visitAllTreeViews: function (callback) {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.visitTreeView(thisTree, callback);
            });
        },
        visitTreeView: function(tree, callback) {
            var domTree = $("[data-sc-id='" + tree.attributes.name + "']");
            if (domTree.length == 0)
                return;
            $.each(domTree, function (key, thisTree) {
                var rootNode = $(thisTree).dynatree("getRoot");
                rootNode.visit(callback);
            });
        },
        expandTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (!node.isExpanded()) {
                    if (node.isLazy()) {
                        node.toggleExpand();
                    }
                    else
                        node.expand(true);
                }
            });
        },
        expandAllTreeViews: function (callback, hideLoading) {

            if (hideLoading == undefined || hideLoading)
                componentDownloaderDialog.showLoading();

            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.expandTreeView(thisTree);
            });

            window.setTimeout(function () {
                $.each(trees, function (key, thisTree) {
                    componentDownloaderDialog.expandTreeView(thisTree);
                });

                window.setTimeout(function () {
                    $.each(trees, function (key, thisTree) {
                        componentDownloaderDialog.expandTreeView(thisTree);
                    });
                    window.scrollTo(0, 0);

                    if (callback != undefined)
                        window.setTimeout(callback, 500);
                    else if (hideLoading == undefined || hideLoading)
                        componentDownloaderDialog.hideLoading();

                }, 500);

            }, 500);

        },
        collapseAllTreeViews: function () {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.collapseTreeView(thisTree);
            });
        },
        collapseTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.expand(false);
                }
            });
        },
        checkAllTreeViews: function () {

            componentDownloaderDialog.showLoading();

            this.expandAllTreeViews(function() {
                var trees = componentDownloaderDialog.getAllTreeViews();
                $.each(trees, function (key, thisTree) {
                    componentDownloaderDialog.checkTreeView(thisTree);
                });
                componentDownloaderDialog.hideLoading();
            },true);
        },
        checkTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.select(true);
                }
            });
        },
        uncheckAllTreeViews: function () {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.uncheckTreeView(thisTree);
            });
        },
        uncheckTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.select(false);
                }
            });
        },
        getRenderingId: function () {
            var id = Sitecore.Helpers.url.getQueryParameters(window.location.href)['cid'];
            if (Sitecore.Helpers.id.isId(id)) {
                return id;
            }
            return null;
        },
        getTreeSourceName: function(tree) {
            return tree.viewModel.$el.attr("data-source");
        },
        getTreeSelectedNodes: function (tree) {
            var selectedNodes = new Array();
            componentDownloaderDialog.visitTreeView(tree, function (node) {
                if (node.isSelected()) {
                    selectedNodes[selectedNodes.length] = node;
                }
            });
            return selectedNodes;
        },
        getSelectedSourcesAndPaths: function () {
            var trees = this.getAllTreeViews();
            var selectedNodes = new Array();
            $.each(trees, function (key, thisTree) {
                var treeSourceName = componentDownloaderDialog.getTreeSourceName(thisTree);
                var treeSelectedNodes = componentDownloaderDialog.getTreeSelectedNodes(thisTree);
                var selectedPathsAndDbs = new Array();              
                for (var i = 0; i < treeSelectedNodes.length; i++) {
                    selectedPathsAndDbs[selectedPathsAndDbs.length] = {
                        path: treeSelectedNodes[i].data.path,
                        database: treeSelectedNodes[0].data.itemUri.databaseUri.databaseName
                    }
                }
                selectedNodes[selectedNodes.length] = {
                    sourceName: treeSourceName,
                    pathsAndDbs: selectedPathsAndDbs
                };
            });
            return selectedNodes;
        },
        download: function (a, b, c) {
            //var selectedNodes = this.getSelectedNodes();

            //alert(selectedNodes.length);
            debugger;

            var getSelectedSourcesAndPaths = this.getSelectedSourcesAndPaths();
            var jsonToSend = JSON.stringify(getSelectedSourcesAndPaths);

            // data-sc-id="DownloadForm"
            var form = $("[data-sc-id='DownloadForm']");
            if (form.length === 0)
                return;
            form.attr("method","POST");

            $("<input/>",
            {
                id: "selectedPaths",
                name: "selectedPaths",
                type: "hidden",
                value: jsonToSend
            }).appendTo(form);
            $("<input/>",
            {
                id: "renderingId",
                name: "renderingId",
                type: "hidden",
                value: this.getRenderingId()
            }).appendTo(form);

            form.submit();
        }
    });
    return ListPageDialog;
});